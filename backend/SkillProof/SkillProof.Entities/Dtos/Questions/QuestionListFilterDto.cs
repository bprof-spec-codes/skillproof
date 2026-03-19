using SkillProof.Entities.Enums;

namespace SkillProof.Entities.Dtos.Questions
{
    public class QuestionListFilterDto
    {
        public QuestionType? Type { get; set; }
        public DifficultyLevel? Difficulty { get; set; }
        public string? Language { get; set; }
        public bool? IsActive { get; set; }
    }
}
