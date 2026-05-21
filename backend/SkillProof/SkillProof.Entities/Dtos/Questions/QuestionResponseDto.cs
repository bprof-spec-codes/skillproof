using SkillProof.Entities.Enums;

namespace SkillProof.Entities.Dtos.Questions
{
    public class QuestionResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public string Language { get; set; } = string.Empty;
        public DifficultyLevel Difficulty { get; set; }
        public List<string> Tags { get; set; } = new();
        public string Title { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public MultipleChoiceQuestionPayloadDto? MultipleChoice { get; set; }
        public CodeCompletionQuestionPayloadDto? CodeCompletion { get; set; }
        public FillInTheBlankQuestionPayloadDto? FillInTheBlank { get; set; }
        public TrueFalseQuestionPayloadDto? TrueFalse { get; set; }
    }
}
