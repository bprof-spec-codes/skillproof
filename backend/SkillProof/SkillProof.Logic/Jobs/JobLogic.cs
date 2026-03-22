using Microsoft.EntityFrameworkCore;
using SkillProof.Data.Repositorys;
using SkillProof.Entities.Dtos.Job;
using SkillProof.Entities.Models;

namespace SkillProof.Logic.Jobs;

public class JobLogic : IJobLogic
{
    private readonly IRepository<Job> _jobRepository; 

    public JobLogic(IRepository<Job> jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<Job> CreateJobAsync(JobCreateDto model, string companyId)
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

        return newJob;
    }
    
    public async Task<IEnumerable<Job>> GetAllJobsAsync()
    {
        return await _jobRepository.GetAll().ToListAsync();
    }

    public async Task<Job?> GetJobByIdAsync(string id)
    {
        return await _jobRepository.GetOne(id);
    }

    public async Task<Job> UpdateJobAsync(string id, JobCreateDto model, string companyId)
    {
        var job = await _jobRepository.GetOne(id);
        if (job == null)
        {
            throw new Exception("The job is not found.");
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

        return await _jobRepository.Update(job);
    }

    public async Task DeleteJobAsync(string id, string companyId)
    {
        var job = await _jobRepository.GetOne(id);
        if (job == null)
        {
            throw new Exception("The job is not found.");
        }

        if (job.CompanyId != companyId)
        {
            throw new UnauthorizedAccessException("You do not have permission to modify this job.\n");
        }

        await _jobRepository.DeleteById(id);
    }
}