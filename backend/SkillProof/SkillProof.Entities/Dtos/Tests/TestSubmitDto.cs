namespace SkillProof.Entities.Dtos.Tests;

public class TestSubmitDto
{
    public string JobId { get; set; } = string.Empty;
    public List<TestAnswerSubmitDto> Answers { get; set; } = new();
}
