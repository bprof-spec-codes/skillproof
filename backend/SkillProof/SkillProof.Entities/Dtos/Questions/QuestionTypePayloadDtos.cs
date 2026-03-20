using System.ComponentModel.DataAnnotations;

namespace SkillProof.Entities.Dtos.Questions
{
    public class MultipleChoiceQuestionPayloadDto
    {
        [Required]
        [MinLength(1)]
        public List<string> Options { get; set; } = new();

        [Required]
        [MinLength(1)]
        public List<int> CorrectOptionIndexes { get; set; } = new();

        public bool AllowMultipleSelection { get; set; }
    }

    public class CodeCompletionQuestionPayloadDto
    {
        [Required]
        public string CodeSnippet { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public List<string> AcceptedAnswers { get; set; } = new();
    }

    public class FillInTheBlankQuestionPayloadDto
    {
        [Required]
        public string Answer { get; set; } = string.Empty;

        public string? ManualFeedback { get; set; }
    }

    public class TrueFalseQuestionPayloadDto
    {
        public bool CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
    }
}
