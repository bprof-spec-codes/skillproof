using SkillProof.Entities.Enums;

namespace SkillProof.Entities.Dtos.Tests;

public class CandidateAssessmentDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DifficultyLevel DifficultyLevel { get; set; }
    public List<CandidateQuestionDto> Questions { get; set; } = new();
}
