using SkillProof.Entities.Enums;

namespace SkillProof.Entities.Dtos.Tests;

public class QuestionResultDto
{
    public string QuestionId { get; set; } = string.Empty;
    public string QuestionTitle { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public bool IsCorrect { get; set; }
    public int PointsAwarded { get; set; }
    public int MaxPoints { get; set; }
    public string UserResponse { get; set; } = string.Empty;
    public string AiFeedback { get; set; } = string.Empty;
}
