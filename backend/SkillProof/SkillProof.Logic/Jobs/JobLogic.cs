using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillProof.Data.Repositorys;
using SkillProof.Entities.Dtos.Assesment;
using SkillProof.Entities.Dtos.Job;
using SkillProof.Entities.Dtos.Jobs;
using SkillProof.Entities.Dtos.Questions;
using SkillProof.Entities.Models;
using SkillProof.Logic.Helper;

namespace SkillProof.Logic.Jobs;

public class JobLogic : IJobLogic
{
    private readonly IRepository<Job> _jobRepository;
    private readonly IRepository<Entities.Models.Assessments> _assessmentRepository;
    private readonly UserManager<Users> _userManager;
    private readonly IRepository<SkillProof.Entities.Models.Questions> _questionRepository;
    private readonly IMarkdownService _markdownService;

    public JobLogic(
        IRepository<Job> jobRepository,
        IRepository<Entities.Models.Assessments> assessmentRepository,
        UserManager<Users> userManager,
        IMarkdownService markdownService)
    {
        _jobRepository = jobRepository;
        _assessmentRepository = assessmentRepository;
        _userManager = userManager;
        _markdownService = markdownService;
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

    /*public async Task<List<QuestionResponseDto>> GetRndQuestions(string id)
    {
        Job job = await _jobRepository.GetOne(id);

        string tagsOfJob = job.Tags;


    }
    */
}