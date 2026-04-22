using SkillProof.Entities.Enums;

namespace SkillProof.Entities.Dtos.Tests;

public class CandidateQuestionDto
{
    public string Id { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }

    public CandidateMultipleChoicePayloadDto? MultipleChoice { get; set; }
    public CandidateCodeCompletionPayloadDto? CodeCompletion { get; set; }
    public CandidateFillInTheBlankPayloadDto? FillInTheBlank { get; set; }
    public CandidateTrueFalsePayloadDto? TrueFalse { get; set; }
}
