using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillProof.Data;
using SkillProof.Data.Repositorys;
using SkillProof.Entities.Dtos.Assesment;
using SkillProof.Entities.Dtos.Job;
using SkillProof.Entities.Dtos.Jobs;
using SkillProof.Entities.Dtos.Questions;
using SkillProof.Entities.Dtos.Tests;
using SkillProof.Entities.Enums;
using SkillProof.Entities.Models;
using SkillProof.Logic.Helper;
using System.Linq;
using System.Text.Json;

namespace SkillProof.Logic.Jobs;

public class JobLogic : IJobLogic
{
    private readonly IRepository<Job> _jobRepository;
    private readonly IRepository<Entities.Models.Assessments> _assessmentRepository;
    private readonly UserManager<Users> _userManager;
    private readonly IRepository<SkillProof.Entities.Models.Questions> _questionRepository;
    private readonly IMarkdownService _markdownService;
    private readonly SkillProofDbContext _ctx;

    public JobLogic(
        IRepository<Job> jobRepository,
        IRepository<Entities.Models.Assessments> assessmentRepository,
        UserManager<Users> userManager,
        IMarkdownService markdownService, SkillProofDbContext ctx)
    {
        _jobRepository = jobRepository;
        _assessmentRepository = assessmentRepository;
        _userManager = userManager;
        _markdownService = markdownService;
        _ctx = ctx;
    }

    public async Task<JobViewDto> CreateJobAsync(JobCreateDto model, string companyId)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "The job data model cannot be null.");
        }

        var newJob = new Job
        {
            Id = Guid.NewGuid().ToString(),
            CompanyId = companyId,
            Title = model.Title,
            Description = _markdownService.ToHtml(model.Description),
            ShortDescription = model.ShortDescription,
            Location = model.Location,
            Tags = model.Tags,
            EmploymentType = model.EmploymentType,
            CreatedAt = DateTime.UtcNow
        };

        if (model.AssessmentIds != null && model.AssessmentIds.Any())
        {
            var assessments = await _assessmentRepository.GetAll()
                .Where(a => model.AssessmentIds.Contains(a.Id))
                .ToListAsync();

            foreach (var assessment in assessments)
            {
                newJob.Assessments.Add(assessment);
            }
        }

        await _jobRepository.Create(newJob);

        return new JobViewDto
        {
            CompanyId = newJob.CompanyId,
            Title = newJob.Title,
            Description = newJob.Description,
            ShortDescription = newJob.ShortDescription,
            Location = newJob.Location,
            Tags = newJob.Tags,
            EmploymentType = newJob.EmploymentType,
            CreatedAt = newJob.CreatedAt,
            Id = newJob.Id,
            AssessmentIds = newJob.Assessments.Select(a => a.Id).ToList()
        };
    }

    public async Task<IEnumerable<JobViewDto>> GetAllJobsAsync()
    {
        return await _jobRepository.GetAll()
            .Include(j => j.Assessments)
                .ThenInclude(a => a.Questions)
            .Select(j => new JobViewDto
            {
                CompanyId = j.CompanyId,
                Title = j.Title,
                Description = j.Description,
                ShortDescription = j.ShortDescription,
                Location = j.Location,
                Tags = j.Tags,
                EmploymentType = j.EmploymentType,
                CreatedAt = j.CreatedAt,
                Id = j.Id,
                AssessmentIds = j.Assessments.Select(a => a.Id).ToList(),
                Assessments = j.Assessments.Select(a => new AssessmentViewDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    DifficultyLevel = a.DifficultyLevel,
                    CreatedBy = a.CreatedBy,
                    CreatedAt = a.CreatedAt,
                    IsActive = a.IsActive,
                    QuestionIds = a.Questions.Select(q => q.Id).ToList(),
                    Questions = a.Questions.Select(q => new QuestionResponseDto
                    {
                        Id = q.Id,
                        Title = q.Title,
                        Type = q.Type,
                        Difficulty = q.Difficulty,
                        Language = q.Language
                    }).ToList()
                }).ToList()
            }).ToListAsync();
    }

    public async Task<JobViewDto?> GetJobByIdAsync(string id)
    {
        var job = await _jobRepository.GetAll()
            .Include(j => j.Assessments)
                .ThenInclude(a => a.Questions)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
        {
            throw new KeyNotFoundException("The job is not found.");
        }

        return new JobViewDto
        {
            CompanyId = job.CompanyId,
            Title = job.Title,
            Description = job.Description,
            ShortDescription = job.ShortDescription,
            Location = job.Location,
            Tags = job.Tags,
            EmploymentType = job.EmploymentType,
            CreatedAt = job.CreatedAt,
            Id = job.Id,
            AssessmentIds = job.Assessments.Select(a => a.Id).ToList(),
            Assessments = job.Assessments.Select(a => new AssessmentViewDto
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                DifficultyLevel = a.DifficultyLevel,
                CreatedBy = a.CreatedBy,
                CreatedAt = a.CreatedAt,
                IsActive = a.IsActive,
                QuestionIds = a.Questions.Select(q => q.Id).ToList(),
                Questions = a.Questions.Select(q => new QuestionResponseDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Type = q.Type,
                    Difficulty = q.Difficulty,
                    Language = q.Language
                }).ToList()
            }).ToList()
        };
    }

    public async Task<IEnumerable<JobViewDto>> GetJobsByCompanyIdAsync(string companyId)
    {
        if (string.IsNullOrWhiteSpace(companyId))
        {
            throw new ArgumentException("Company ID cannot be null or empty.", nameof(companyId));
        }

        return await _jobRepository.GetAll()
            .Include(j => j.Assessments)
                .ThenInclude(a => a.Questions)
            .Where(j => j.CompanyId == companyId)
            .Select(j => new JobViewDto
            {
                CompanyId = j.CompanyId,
                Title = j.Title,
                Description = j.Description,
                ShortDescription = j.ShortDescription,
                Location = j.Location,
                Tags = j.Tags,
                EmploymentType = j.EmploymentType,
                CreatedAt = j.CreatedAt,
                Id = j.Id,
                AssessmentIds = j.Assessments.Select(a => a.Id).ToList(),
                Assessments = j.Assessments.Select(a => new AssessmentViewDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    DifficultyLevel = a.DifficultyLevel,
                    CreatedBy = a.CreatedBy,
                    CreatedAt = a.CreatedAt,
                    IsActive = a.IsActive,
                    QuestionIds = a.Questions.Select(q => q.Id).ToList(),
                    Questions = a.Questions.Select(q => new QuestionResponseDto
                    {
                        Id = q.Id,
                        Title = q.Title,
                        Type = q.Type,
                        Difficulty = q.Difficulty,
                        Language = q.Language
                    }).ToList()
                }).ToList()
            }).ToListAsync();
    }

    public async Task<JobViewDto> UpdateJobAsync(string id, JobViewDto model, string companyId)
    {
        var job = await _jobRepository.GetAll()
            .Include(j => j.Assessments)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
        {
            throw new KeyNotFoundException("The job is not found.");
        }

        if (job.CompanyId != companyId)
        {
            throw new UnauthorizedAccessException("You do not have permission to modify this job.");
        }

        job.Title = model.Title;
        job.Description = model.Description;
        job.ShortDescription = model.ShortDescription;
        job.Location = model.Location;
        job.Tags = model.Tags;
        job.EmploymentType = model.EmploymentType;

        job.Assessments.Clear();

        if (model.AssessmentIds != null && model.AssessmentIds.Any())
        {
            var selectedAssessments = await _assessmentRepository.GetAll()
                .Where(a => model.AssessmentIds.Contains(a.Id))
                .ToListAsync();

            foreach (var assessment in selectedAssessments)
            {
                job.Assessments.Add(assessment);
            }
        }

        await _jobRepository.Update(job);

        return new JobViewDto
        {
            CompanyId = job.CompanyId,
            Title = job.Title,
            Description = job.Description,
            ShortDescription = job.ShortDescription,
            Location = job.Location,
            Tags = job.Tags,
            EmploymentType = job.EmploymentType,
            CreatedAt = job.CreatedAt,
            Id = job.Id,
            AssessmentIds = job.Assessments.Select(a => a.Id).ToList()
        };
    }

    public async Task DeleteJobAsync(string id, string currentUserId)
    {
        var job = await _jobRepository.GetOne(id);
        if (job == null)
        {
            throw new KeyNotFoundException("The job is not found.");
        }

        var user = await _userManager.FindByIdAsync(currentUserId);
        if (user == null || user.CompanyId == null)
        {
            throw new UnauthorizedAccessException("User profile not found or not associated with a company.");
        }

        if (job.CompanyId != user.CompanyId)
        {
            throw new UnauthorizedAccessException("You do not have permission to modify a job belonging to another company.");
        }

        if (user.CompanyRole != "Owner")
        {
            throw new UnauthorizedAccessException("Only the Company Owner can perform this action.");
        }

        await _jobRepository.DeleteById(id);
    }

    public async Task<ICollection<AssessmentViewDto>> GetTestToJob(string id)
    {
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
        .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
            return new List<AssessmentViewDto>();

        return job.Assessments.Select(a => new AssessmentViewDto
        {
            Id = a.Id,
            Title = a.Title,
            Description = a.Description,
            DifficultyLevel = a.DifficultyLevel,
            CreatedBy = a.CreatedBy,
            CreatedAt = a.CreatedAt,
            IsActive = a.IsActive,

            QuestionIds = a.Questions.Select(q => q.Id).ToList(),

            Questions = a.Questions.Select(q => new QuestionResponseDto
            {
                Id = q.Id,
                Title = q.Title,
                Type = q.Type,
                Difficulty = q.Difficulty,
                Language = q.Language,

                QuestionText = q.QuestionText,
                CreatedBy = q.CreatedBy,
                CreatedAt = q.CreatedAt,
                IsActive = q.IsActive,
                UpdatedAt = q.UpdatedAt,

                MultipleChoice = q.MultipleChoiceQuestion == null ? null : new MultipleChoiceQuestionPayloadDto
                {
                    Options = string.IsNullOrWhiteSpace(q.MultipleChoiceQuestion.Options)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(q.MultipleChoiceQuestion.Options) ?? new List<string>(),

                                CorrectOptionIndexes = string.IsNullOrWhiteSpace(q.MultipleChoiceQuestion.CorrectAnswerIds)
                    ? new List<int>()
                    : JsonSerializer.Deserialize<List<int>>(q.MultipleChoiceQuestion.CorrectAnswerIds) ?? new List<int>(),

                    AllowMultipleSelection = q.MultipleChoiceQuestion.AllowMultipleSelection
                },

                CodeCompletion = q.CodeCompletionQuestion == null ? null : new CodeCompletionQuestionPayloadDto
                {
                    CodeSnippet = q.CodeCompletionQuestion.CodeSnippet,

                    AcceptedAnswers = string.IsNullOrWhiteSpace(q.CodeCompletionQuestion.AcceptedAnswers)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(q.CodeCompletionQuestion.AcceptedAnswers) ?? new List<string>()
                },

                FillInTheBlank = q.FillInTheBlankQuestions == null ? null : new FillInTheBlankQuestionPayloadDto
                {
                    Answer = q.FillInTheBlankQuestions.Answer,
                    ManualFeedback = q.FillInTheBlankQuestions.manualFeedback
                },

                TrueFalse = q.TrueFalseQuestion == null ? null : new TrueFalseQuestionPayloadDto
                {
                    CorrectAnswer = q.TrueFalseQuestion.CorrectAnswer,
                    Explanation = q.TrueFalseQuestion.Explanation
                }

            }).ToList()

        }).ToList();
    }

    public async Task<CandidateAssessmentDto?> GetCandidateTestForJob(string jobId)
    {
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
            .FirstOrDefaultAsync(j => j.Id == jobId);

        if (job == null || job.Assessments.Count == 0)
        {
            return null;
        }

        var firstAssessment = job.Assessments.First();
        var allQuestions = job.Assessments.SelectMany(a => a.Questions).ToList();

        if (allQuestions.Count == 0)
        {
            return null;
        }

        return new CandidateAssessmentDto
        {
            Id = firstAssessment.Id,
            Title = firstAssessment.Title,
            DifficultyLevel = firstAssessment.DifficultyLevel,
            Questions = allQuestions.Select(q => new CandidateQuestionDto
            {
                Id = q.Id,
                Type = q.Type,
                Title = q.Title,
                QuestionText = q.QuestionText,
                Language = q.Language,
                Difficulty = q.Difficulty,

                MultipleChoice = q.MultipleChoiceQuestion == null ? null : new CandidateMultipleChoicePayloadDto
                {
                    Options = string.IsNullOrWhiteSpace(q.MultipleChoiceQuestion.Options)
                        ? new List<string>()
                        : JsonSerializer.Deserialize<List<string>>(q.MultipleChoiceQuestion.Options) ?? new List<string>(),
                    AllowMultipleSelection = q.MultipleChoiceQuestion.AllowMultipleSelection
                },

                CodeCompletion = q.CodeCompletionQuestion == null ? null : new CandidateCodeCompletionPayloadDto
                {
                    CodeSnippet = q.CodeCompletionQuestion.CodeSnippet
                },

                FillInTheBlank = q.FillInTheBlankQuestions == null ? null : new CandidateFillInTheBlankPayloadDto(),

                TrueFalse = q.TrueFalseQuestion == null ? null : new CandidateTrueFalsePayloadDto()
            }).ToList()
        };
    }


    public async Task<IEnumerable<JobViewDto>> GetJobsOfCompanyAsync(string currentUserId)
    {
        var user = await _userManager.FindByIdAsync(currentUserId);
        if (user == null || user.CompanyId == null)
        {
            throw new UnauthorizedAccessException("User profile not found or not associated with a company.");
        }

        return await _jobRepository.GetAll()
            .Where(j => j.CompanyId == user.CompanyId)
            .Select(j => new JobViewDto
            {
                CompanyId = j.CompanyId,
                Title = j.Title,
                Description = j.Description,
                ShortDescription = j.ShortDescription,
                Location = j.Location,
                Tags = j.Tags,
                EmploymentType = j.EmploymentType,
                CreatedAt = j.CreatedAt,
                Id = j.Id
            })
            .ToListAsync();
    }

    public async Task<string> ApplyForJobAsync(string jobId, string userId)
    {
        var jobExists = await _jobRepository.GetAll().AnyAsync(j => j.Id == jobId);
        if (!jobExists)
        {
            throw new KeyNotFoundException("The job was not found.");
        }

        var existingApplication = await _ctx.JobApplications
            .FirstOrDefaultAsync(ja => ja.JobId == jobId && ja.UserId == userId);

        if (existingApplication != null)
        {
            throw new InvalidOperationException("You have already applied for this job.");
        }

        var jobApplication = new JobApplication
        {
            Id = Guid.NewGuid().ToString(),
            JobId = jobId,
            UserId = userId,
            TestId = null, 
            Status = JobApplicationStatus.Submitted,
            AppliedAt = DateTime.UtcNow
        };

        _ctx.JobApplications.Add(jobApplication);
        await _ctx.SaveChangesAsync();

        return jobApplication.Id;
    }


}