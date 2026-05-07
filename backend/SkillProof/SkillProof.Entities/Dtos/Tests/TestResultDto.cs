using SkillProof.Entities.Enums;

namespace SkillProof.Entities.Dtos.Tests;

public class TestResultDto
{
    public string TestId { get; set; } = string.Empty;
    public string JobApplicationId { get; set; } = string.Empty;
    public double Score { get; set; }
    public int MaxScore { get; set; }
    public bool Passed { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public List<QuestionResultDto> QuestionResults { get; set; } = new();
}
