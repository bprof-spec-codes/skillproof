using Microsoft.AspNetCore.Mvc;
using SkillProof.Entities.Dtos.Assesment;

namespace SkillProof.Logic.Assesments;

public interface IAssessmentLogic
{
    Task<AssessmentViewDto> CreateAssessmentAsync(CreateAssessmentDto model, string userId);
    Task<IEnumerable<AssessmentViewDto>> GetAllAssessmentsAsync();
    Task<AssessmentViewDto?> GetAssessmentByIdAsync(string id);
    Task<AssessmentViewDto> UpdateAssessmentAsync(string id, UpdateAssessmentDto model, string userId);
    Task DeleteAssessmentAsync(string id);

    Task AssignAssessmentToJob(string assessmentId, string jobId);
}