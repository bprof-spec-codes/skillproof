using SkillProof.Entities.Enums;
using SkillProof.Entities.Helper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillProof.Entities.Models
{
    public class Tests: IIdentity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public DifficultyLevel DifficultyLevel { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }


        [Required]
        public bool Passed { get; set; }

        [Required]
        public int Score { get; set; }
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        public virtual JobApplication? JobApplication { get; set; }
        public virtual Users? User { get; set; }
        public virtual ICollection<TestAnswers> TestAnswers { get; set; }
        public virtual ICollection<Assessments> Assessments { get; set; } = new List<Assessments>();
    }
}
