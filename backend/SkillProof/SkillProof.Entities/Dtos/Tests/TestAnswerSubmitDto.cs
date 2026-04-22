namespace SkillProof.Entities.Dtos.Tests;

public class TestAnswerSubmitDto
{
    public string QuestionId { get; set; } = string.Empty;
    public List<int>? SelectedOptionIndexes { get; set; }
    public bool? BoolAnswer { get; set; }
    public string? TextAnswer { get; set; }
}
