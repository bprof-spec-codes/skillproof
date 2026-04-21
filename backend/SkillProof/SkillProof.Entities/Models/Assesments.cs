using SkillProof.Entities.Enums;
using SkillProof.Entities.Helper;
using System.ComponentModel.DataAnnotations;

namespace SkillProof.Entities.Models
{
    public class Assessments : IIdentity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DifficultyLevel DifficultyLevel { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Questions> Questions { get; set; } = new List<Questions>();
        public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
        public virtual ICollection<Tests> TestAttempts { get; set; } = new List<Tests>();
    }
}