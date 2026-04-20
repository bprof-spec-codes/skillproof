using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillProof.Data.Repositorys;
using SkillProof.Entities.Dtos.Job;
using SkillProof.Entities.Dtos.Jobs;
using SkillProof.Entities.Models;
using SkillProof.Logic.Helper;

namespace SkillProof.Logic.Jobs;

public class JobLogic : IJobLogic
{
    private readonly IRepository<Job> _jobRepository;
    private readonly UserManager<Users> _userManager;
    private readonly IMarkdownService _markdownService;

    public JobLogic(IRepository<Job> jobRepository,UserManager<Users> userManager, IMarkdownService markdownService)
    {
        _jobRepository = jobRepository;
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
            Id = newJob.Id
        };
    }
    
    public async Task<IEnumerable<JobViewDto>> GetAllJobsAsync()
    {
        return await _jobRepository.GetAll().Select(j => new JobViewDto{
            CompanyId = j.CompanyId,
            Title = j.Title,
            Description = j.Description,
            Location = j.Location,
            Tags = j.Tags,
            EmploymentType = j.EmploymentType,
            CreatedAt= j.CreatedAt,
            Id = j.Id
        }).ToListAsync();
    }

    public async Task<JobViewDto?> GetJobByIdAsync(string id)
    {
        var job = await _jobRepository.GetOne(id);
        if(job == null)
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
            CreatedAt= job.CreatedAt,
            Id = job.Id
        };
    }

    public async Task<JobViewDto> UpdateJobAsync(string id, JobViewDto model, string companyId)
    {
        var job = await _jobRepository.GetOne(id);
        if (job == null)
        {
            throw new KeyNotFoundException("The job is not found.");
        }

        if (job.CompanyId != companyId)
        {
            throw new UnauthorizedAccessException("You do not have permission to modify this job.\n");
        }

        job.Title = model.Title;
        job.Description = model.Description;
        job.Location = model.Location;
        job.Tags = model.Tags;
        job.EmploymentType = model.EmploymentType;

        await _jobRepository.Update(job);

        return new JobViewDto
        {
            CompanyId = job.CompanyId,
            Title = job.Title,
            Description = job.Description,
            Location = job.Location,
            Tags = job.Tags,
            EmploymentType = job.EmploymentType,
            CreatedAt= job.CreatedAt,
            Id = job.Id
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
}