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
}