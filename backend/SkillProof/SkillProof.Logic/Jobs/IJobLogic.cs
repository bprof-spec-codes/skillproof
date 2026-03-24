using SkillProof.Entities.Models;
using SkillProof.Entities.Dtos.Job;
using SkillProof.Entities.Dtos.Jobs;

namespace SkillProof.Logic.Jobs;

public interface IJobLogic
{
    Task<IEnumerable<JobViewDto>> GetAllJobsAsync();
    Task<JobViewDto?> GetJobByIdAsync(string id);
    Task<JobViewDto> UpdateJobAsync(string id, JobCreateDto model, string companyId);
    Task DeleteJobAsync(string id, string companyId);
    Task<JobViewDto> CreateJobAsync(JobCreateDto model, string companyId);
}