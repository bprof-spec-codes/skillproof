using SkillProof.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace SkillProof.Entities.Dtos.Questions
{
    public class CreateQuestionRequestDto
    {
        [Required]
        public QuestionType Type { get; set; }

        [Required]
        [StringLength(20)]
        public string Language { get; set; } = string.Empty;

        [Required]
        public DifficultyLevel Difficulty { get; set; }

        public List<string> Tags { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        [StringLength(450)]
        public string CreatedBy { get; set; } = string.Empty;

        public MultipleChoiceQuestionPayloadDto? MultipleChoice { get; set; }
        public CodeCompletionQuestionPayloadDto? CodeCompletion { get; set; }
        public FillInTheBlankQuestionPayloadDto? FillInTheBlank { get; set; }
        public TrueFalseQuestionPayloadDto? TrueFalse { get; set; }
    }
}
