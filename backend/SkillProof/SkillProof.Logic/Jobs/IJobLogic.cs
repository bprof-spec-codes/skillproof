using SkillProof.Entities.Models;
using SkillProof.Entities.Dtos.Job;

namespace SkillProof.Logic.Jobs;

public interface IJobLogic
{
    Task<IEnumerable<Job>> GetAllJobsAsync();
    Task<Job?> GetJobByIdAsync(string id);
    Task<Job> UpdateJobAsync(string id, JobCreateDto model, string companyId);
    Task DeleteJobAsync(string id, string companyId);
    Task<Job> CreateJobAsync(JobCreateDto model, string companyId);
}