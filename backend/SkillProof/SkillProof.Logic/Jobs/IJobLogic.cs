using SkillProof.Entities.Models;
using SkillProof.Entities.Dtos.Job;

namespace SkillProof.Logic.Jobs;

public interface IJobLogic
{
    Task<Job> CreateJobAsync(JobCreateDto model, string companyId);
}