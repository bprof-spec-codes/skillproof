using SkillProof.Entities.Enums;

namespace SkillProof.Entities.Dtos.Assesment;

public class CreateAssessmentDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public List<string> QuestionIds { get; set; } = new List<string>();
}