using SkillProof.Entities.Dtos.Assesment;
using SkillProof.Entities.Dtos.Job;
using SkillProof.Entities.Dtos.Jobs;
using SkillProof.Entities.Dtos.Questions;
using SkillProof.Entities.Dtos.Tests;
using SkillProof.Entities.Models;

namespace SkillProof.Logic.Jobs;

public interface IJobLogic
{
    Task<IEnumerable<JobViewDto>> GetAllJobsAsync();
    Task<JobViewDto?> GetJobByIdAsync(string id);
    Task<IEnumerable<JobViewDto>> GetJobsByCompanyIdAsync(string companyId);
    Task<JobViewDto> UpdateJobAsync(string id, JobViewDto model, string companyId);
    Task DeleteJobAsync(string id, string companyId);
    Task<JobViewDto> CreateJobAsync(JobCreateDto model, string companyId);

    Task<ICollection<AssessmentViewDto>> GetTestToJob(string id);

    Task<CandidateAssessmentDto?> GetCandidateTestForJob(string jobId);

}