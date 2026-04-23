namespace SkillProof.Entities.Dtos.Tests;

public class CandidateMultipleChoicePayloadDto
{
    public List<string> Options { get; set; } = new();
    public bool AllowMultipleSelection { get; set; }
}
