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

namespace SkillProof.Logic.Tests;

public class TestLogic : ITestLogic
{
    private const string AiFeedbackPending = "Auto-scored (AI integration pending)";
    private const string AiFeedbackNotApplicable = "-";

    private readonly IRepository<Job> _jobRepository;
    private readonly SkillProofDbContext _ctx;

    public TestLogic(IRepository<Job> jobRepository, SkillProofDbContext ctx)
    {
        _jobRepository = jobRepository;
        _ctx = ctx;
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

        var questionResults = new List<QuestionResultDto>();
        var totalScore = 0;

        foreach (var question in allQuestions)
        {
            answersByQuestionId.TryGetValue(question.Id, out var submitted);

            var scored = ScoreQuestion(question, submitted);

            var answerEntity = new TestAnswerEntity
            {
                Id = Guid.NewGuid().ToString(),
                QuestionId = question.Id,
                TestId = test.Id,
                FreeTextResponse = scored.FreeTextResponse,
                IsCorrect = scored.IsCorrect,
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

    private sealed record ScoredAnswer(int Points, bool IsCorrect, string FreeTextResponse, string AiFeedback);

    private static ScoredAnswer ScoreQuestion(QuestionEntity question, TestAnswerSubmitDto? submitted)
    {
        return question.Type switch
        {
            QuestionType.MultipleChoice => ScoreMultipleChoice(question, submitted),
            QuestionType.TrueFalse => ScoreTrueFalse(question, submitted),
            QuestionType.CodeCompletion => ScoreCodeCompletion(question, submitted),
            QuestionType.FillInTheBlank => ScoreFillInTheBlank(submitted),
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

    private static ScoredAnswer ScoreFillInTheBlank(TestAnswerSubmitDto? submitted)
    {
        var text = submitted?.TextAnswer ?? string.Empty;
        return new ScoredAnswer(1, true, text, AiFeedbackPending);
    }
}
