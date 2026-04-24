using SkillProof.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace SkillProof.Entities.Dtos.Questions
{
    public class UpdateQuestionRequestDto
    {
        [StringLength(20)]
        public string? Language { get; set; }

        [Required]
        public DifficultyLevel Difficulty { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public MultipleChoiceQuestionPayloadDto? MultipleChoice { get; set; }
        public CodeCompletionQuestionPayloadDto? CodeCompletion { get; set; }
        public FillInTheBlankQuestionPayloadDto? FillInTheBlank { get; set; }
        public TrueFalseQuestionPayloadDto? TrueFalse { get; set; }
    }
}
