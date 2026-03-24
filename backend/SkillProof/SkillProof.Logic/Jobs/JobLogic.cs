using Microsoft.EntityFrameworkCore;
using SkillProof.Data.Repositorys;
using SkillProof.Entities.Dtos.Job;
using SkillProof.Entities.Dtos.Jobs;
using SkillProof.Entities.Models;

namespace SkillProof.Logic.Jobs;

public class JobLogic : IJobLogic
{
    private readonly IRepository<Job> _jobRepository; 

    public JobLogic(IRepository<Job> jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<JobViewDto> CreateJobAsync(JobCreateDto model, string companyId)
    {
        var newJob = new Job
        {
            Id = Guid.NewGuid().ToString(),
            CompanyId = companyId, 
            Title = model.Title,
            Description = model.Description,
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

    public async Task<JobViewDto> UpdateJobAsync(string id, JobCreateDto model, string companyId)
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

    public async Task DeleteJobAsync(string id, string companyId)
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

        await _jobRepository.DeleteById(id);
    }
}