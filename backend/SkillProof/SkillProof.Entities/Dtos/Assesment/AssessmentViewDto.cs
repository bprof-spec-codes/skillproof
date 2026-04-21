using SkillProof.Entities.Dtos.Questions;
using SkillProof.Entities.Enums;

namespace SkillProof.Entities.Dtos.Assesment;

public class AssessmentViewDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
        
    public List<QuestionResponseDto> Questions { get; set; } = new List<QuestionResponseDto>();
    public List<string> QuestionIds { get; set; } = new List<string>();
}