using Microsoft.EntityFrameworkCore;
using SkillProof.Data;
using SkillProof.Data.Repositorys;
using SkillProof.Entities.Dtos.Tests;
using SkillProof.Entities.Enums;
using SkillProof.Entities.Models;
using System.Text.Json;
using QuestionEntity = SkillProof.Entities.Models.Questions;
using TestEntity = SkillProof.Entities.Models.Tests;
using TestAnswerEntity = SkillProof.Entities.Models.TestAnswers;
using SkillProof.Logic.Gemini;
using SkillProof.Entities.Models.Gemini;

namespace SkillProof.Logic.Tests;

public class TestLogic : ITestLogic
{
    private const string AiFeedbackPending = "Auto-scored (AI integration pending)";
    private const string AiFeedbackNotApplicable = "-";

    private readonly IRepository<Job> _jobRepository;
    private readonly SkillProofDbContext _ctx;
    private readonly IGeminiService _geminiService;

    public TestLogic(IRepository<Job> jobRepository, SkillProofDbContext ctx, IGeminiService geminiService)
    {
        _jobRepository = jobRepository;
        _ctx = ctx;
        _geminiService = geminiService;
    }

    public async Task<TestResultDto> SubmitTestAsync(TestSubmitDto dto, string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException("A user identity is required to submit a test.");
        }

        if (dto == null || string.IsNullOrWhiteSpace(dto.JobId))
        {
            throw new ArgumentException("Job id is required.");
        }

        if (dto.Answers == null)
        {
            throw new ArgumentException("Answers must be provided (may be empty per question, but the list itself is required).");
        }

        var duplicateIds = dto.Answers
            .GroupBy(a => a.QuestionId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateIds.Count > 0)
        {
            throw new ArgumentException($"Duplicate answers provided for question(s): {string.Join(", ", duplicateIds)}.");
        }

        var job = await _jobRepository.GetAll()
            .Include(j => j.Assessments)
                .ThenInclude(a => a.Questions)
                    .ThenInclude(q => q.MultipleChoiceQuestion)
            .Include(j => j.Assessments)
                .ThenInclude(a => a.Questions)
                    .ThenInclude(q => q.CodeCompletionQuestion)
            .Include(j => j.Assessments)
                .ThenInclude(a => a.Questions)
                    .ThenInclude(q => q.FillInTheBlankQuestions)
            .Include(j => j.Assessments)
                .ThenInclude(a => a.Questions)
                    .ThenInclude(q => q.TrueFalseQuestion)
            .Include(j => j.Assessments)
                .ThenInclude(t => t.TestAttempts)
            .FirstOrDefaultAsync(j => j.Id == dto.JobId);

        if (job == null)
        {
            throw new KeyNotFoundException($"Job '{dto.JobId}' not found.");
        }

        if (job.Assessments.Count == 0)
        {
            throw new ArgumentException("This job has no assessment attached.");
        }

        var allQuestions = job.Assessments.SelectMany(a => a.Questions).ToList();

        if (allQuestions.Count == 0)
        {
            throw new ArgumentException("The assessments attached to this job have no questions.");
        }

        var questionIdSet = allQuestions.Select(q => q.Id).ToHashSet();
        var foreignIds = dto.Answers
            .Where(a => !questionIdSet.Contains(a.QuestionId))
            .Select(a => a.QuestionId)
            .ToList();

        if (foreignIds.Count > 0)
        {
            throw new ArgumentException($"Answer(s) provided for question(s) that do not belong to this job's test: {string.Join(", ", foreignIds)}.");
        }

        var answersByQuestionId = dto.Answers.ToDictionary(a => a.QuestionId, a => a);

        var firstAssessment = job.Assessments.First();
        var test = new TestEntity
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            DifficultyLevel = firstAssessment.DifficultyLevel,
            CompletedAt = DateTime.UtcNow,
            Score = 0,
            Passed = false,
            TestAnswers = new List<TestAnswerEntity>()
        };
        job.Assessments.First().TestAttempts.Add(test);

        var questionResults = new List<QuestionResultDto>();
        double totalScore = 0;

        foreach (var question in allQuestions)
        {
            answersByQuestionId.TryGetValue(question.Id, out var submitted);

            var scored = ScoreQuestion(question, submitted, _geminiService);

            var answerEntity = new TestAnswerEntity
            {
                Id = Guid.NewGuid().ToString(),
                QuestionId = question.Id,
                TestId = test.Id,
                FreeTextResponse = scored.FreeTextResponse,
                IsCorrect = scored.IsCorrect,
                Score = scored.Points,
                Inspected = false,
                AiFeedback = scored.AiFeedback
            };

            test.TestAnswers.Add(answerEntity);
            totalScore += scored.Points;

            questionResults.Add(new QuestionResultDto
            {
                QuestionId = question.Id,
                QuestionTitle = question.Title,
                Type = question.Type,
                IsCorrect = scored.IsCorrect,
                PointsAwarded = scored.Points,
                MaxPoints = 1,
                UserResponse = scored.FreeTextResponse,
                AiFeedback = scored.AiFeedback
            });
        }

        test.Score = totalScore;
        test.Passed = totalScore * 2 >= allQuestions.Count;

        var existingApplication = await _ctx.JobApplications
            .FirstOrDefaultAsync(ja => ja.JobId == dto.JobId && ja.UserId == userId);

        JobApplication jobApplication;
        if (existingApplication == null)
        {
            jobApplication = new JobApplication
            {
                Id = Guid.NewGuid().ToString(),
                JobId = dto.JobId,
                UserId = userId,
                TestId = test.Id,
                Status = JobApplicationStatus.TestCompleted,
                AppliedAt = DateTime.UtcNow
            };
            _ctx.JobApplications.Add(jobApplication);
        }
        else
        {
            existingApplication.TestId = test.Id;
            existingApplication.Status = JobApplicationStatus.TestCompleted;
            jobApplication = existingApplication;
        }
        
        _ctx.Tests.Add(test);
        await _ctx.SaveChangesAsync();

        return new TestResultDto
        {
            TestId = test.Id,
            JobApplicationId = jobApplication.Id,
            Score = test.Score,
            MaxScore = allQuestions.Count,
            Passed = test.Passed,
            DifficultyLevel = test.DifficultyLevel,
            QuestionResults = questionResults
        };
    }

    private sealed record ScoredAnswer(double Points, bool IsCorrect, string FreeTextResponse, string AiFeedback);

    private static ScoredAnswer ScoreQuestion(QuestionEntity question, TestAnswerSubmitDto? submitted, IGeminiService geminiService)
    {
        return question.Type switch
        {
            QuestionType.MultipleChoice => ScoreMultipleChoice(question, submitted),
            QuestionType.TrueFalse => ScoreTrueFalse(question, submitted),
            QuestionType.CodeCompletion => ScoreCodeCompletion(question, submitted),
            QuestionType.FillInTheBlank => ScoreFillInTheBlank(question, submitted, geminiService),
            _ => new ScoredAnswer(0, false, string.Empty, AiFeedbackNotApplicable)
        };
    }

    private static ScoredAnswer ScoreMultipleChoice(QuestionEntity question, TestAnswerSubmitDto? submitted)
    {
        var selected = submitted?.SelectedOptionIndexes ?? new List<int>();
        var serialized = JsonSerializer.Serialize(selected);

        if (question.MultipleChoiceQuestion == null)
        {
            return new ScoredAnswer(0, false, serialized, AiFeedbackNotApplicable);
        }

        var correctRaw = question.MultipleChoiceQuestion.CorrectAnswerIds;
        var correct = string.IsNullOrWhiteSpace(correctRaw)
            ? new List<int>()
            : JsonSerializer.Deserialize<List<int>>(correctRaw) ?? new List<int>();

        var isCorrect = selected.ToHashSet().SetEquals(correct);
        return new ScoredAnswer(isCorrect ? 1 : 0, isCorrect, serialized, AiFeedbackNotApplicable);
    }

    private static ScoredAnswer ScoreTrueFalse(QuestionEntity question, TestAnswerSubmitDto? submitted)
    {
        var value = submitted?.BoolAnswer;
        var serialized = value.HasValue ? (value.Value ? "true" : "false") : string.Empty;

        if (question.TrueFalseQuestion == null || !value.HasValue)
        {
            return new ScoredAnswer(0, false, serialized, AiFeedbackNotApplicable);
        }

        var isCorrect = value.Value == question.TrueFalseQuestion.CorrectAnswer;
        return new ScoredAnswer(isCorrect ? 1 : 0, isCorrect, serialized, AiFeedbackNotApplicable);
    }

    private static ScoredAnswer ScoreCodeCompletion(QuestionEntity question, TestAnswerSubmitDto? submitted)
    {
        var text = submitted?.TextAnswer ?? string.Empty;

        if (question.CodeCompletionQuestion == null)
        {
            return new ScoredAnswer(0, false, text, AiFeedbackNotApplicable);
        }

        var acceptedRaw = question.CodeCompletionQuestion.AcceptedAnswers;
        var accepted = string.IsNullOrWhiteSpace(acceptedRaw)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(acceptedRaw) ?? new List<string>();

        var trimmed = text.Trim();
        var isCorrect = accepted.Any(a => string.Equals(a?.Trim(), trimmed, StringComparison.Ordinal));
        return new ScoredAnswer(isCorrect ? 1 : 0, isCorrect, text, AiFeedbackNotApplicable);
    }

    private static ScoredAnswer ScoreFillInTheBlank(QuestionEntity question, TestAnswerSubmitDto? submitted, IGeminiService geminiService)
    {
        var text = submitted?.TextAnswer ?? string.Empty;
        GradingRequest request = new GradingRequest
        {
            Question = question.QuestionText,
            StudentAnswer = text,
            AnswerToQuestion = question.FillInTheBlankQuestions?.Answer ?? string.Empty
        };
        double score = geminiService.EvaluateAnswerAsync(request).GetAwaiter().GetResult();
        
        bool isCorrect = score >= 0.5;
        return new ScoredAnswer(score, isCorrect, text, AiFeedbackPending);
    }

    public async Task<List<UserTestReviewDto>> GetUserTestQuestionsAsync(string jobId, string userId)
    {
        if (jobId == null || string.IsNullOrWhiteSpace(jobId))
        {
            throw new ArgumentException("Job id is required.");
        }
        var job = await _jobRepository.GetAll()
            .Include(j => j.Assessments)
                .ThenInclude(Assessments => Assessments.TestAttempts)
                    .ThenInclude(TestAttempts => TestAttempts.TestAnswers)
                        .ThenInclude(TestAnswers => TestAnswers.Question)
                         .ThenInclude(q => q.MultipleChoiceQuestion)
            .FirstOrDefaultAsync(j => j.Id == jobId);

        if (job == null)
        {
            throw new KeyNotFoundException($"Job '{jobId}' not found.");
        }

        if (job.Assessments.Count == 0)
        {
            throw new ArgumentException("This job has no assessment attached.");
        }

        var attempt = job.Assessments.SelectMany(a => a.TestAttempts).FirstOrDefault(a => a.UserId == userId);



        if (attempt == null)
        {
            throw new ArgumentException($"There are no test attempt for the assessments attached to this job with this user: {userId}.");
        }

        var reviews = attempt.TestAnswers.Select(testAnswers => new UserTestReviewDto
        {
            QuestionId = testAnswers.QuestionId,
            TestAnswerId = testAnswers.Id,
            QuestionText = testAnswers.Question.QuestionText,
            UserResponse = testAnswers.FreeTextResponse,
            Score = testAnswers.Score,
            Inspected = testAnswers.Inspected,
            UserId = userId,
            QuestionType = testAnswers.Question.Type
        }).ToList();
        return reviews;
    }

    public async Task<FeedbackResponseDto> ManualFeedbackAsync(string? feedback, double score, string testAnswerId)
    {
        if (testAnswerId == null || string.IsNullOrWhiteSpace(testAnswerId))
        {
            throw new ArgumentException("Test answer id is required.");
        }

        var testAnwser = await _ctx.TestAnswers
             .Include(a => a.Question)
             .FirstOrDefaultAsync(a => a.Id == testAnswerId);

        if (testAnwser == null)
        {
            throw new KeyNotFoundException($"Test answer '{testAnswerId}' not found.");
        }

        if (!(testAnwser.Question.Type == QuestionType.FillInTheBlank))
        {
            throw new ArgumentException("Manual feedback can only be provided for fill-in-the-blank questions.");
        }

        testAnwser.Score = score;
        testAnwser.IsCorrect = score >= 0.5;
        testAnwser.Inspected = true;
        if (!string.IsNullOrWhiteSpace(feedback))
        {
            testAnwser.FreeTextResponse = feedback;
        }

        var result = new FeedbackResponseDto
        {
            TestAnswerId = testAnwser.Id,
            QuestionText = testAnwser.Question.QuestionText,
            UserResponse = testAnwser.FreeTextResponse,
            Score = testAnwser.Score,
            Inspected = testAnwser.Inspected
        };

        await _ctx.SaveChangesAsync();
        return result;
    }

    public async Task<List<string?>> GetTestUsersAsync(string jobId)
    {
        if (jobId == null || string.IsNullOrWhiteSpace(jobId))
        {
            throw new ArgumentException("Job id is required.");
        }
        var job = await _jobRepository.GetAll()
            .Include(j => j.JobApplications) // <-- Changed this
            .FirstOrDefaultAsync(j => j.Id == jobId);

        if (job == null)
        {
            throw new KeyNotFoundException($"Job '{jobId}' not found.");
        }

        // Changed this to look at JobApplications instead of Assessments
        List<string?> userIds = job.JobApplications
            .Select(a => a.UserId)
            .Distinct()
            .ToList();

        return userIds;
    }
}
