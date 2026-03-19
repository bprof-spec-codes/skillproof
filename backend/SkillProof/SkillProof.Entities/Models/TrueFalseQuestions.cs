using SkillProof.Entities.Helper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillProof.Entities.Models
{
    public class TrueFalseQuestions : IIdentity
    {
        [Key]
        [ForeignKey("Question")]
        public string QuestionId { get; set; }

        [NotMapped]
        public string Id
        {
            get => QuestionId;
            set => QuestionId = value;
        }

        [Required]
        public bool CorrectAnswer { get; set; }

        public string? Explanation { get; set; }

        public virtual Questions Question { get; set; }
    }
}
