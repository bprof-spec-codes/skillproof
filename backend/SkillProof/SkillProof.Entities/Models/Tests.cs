using SkillProof.Entities.Enums;
using SkillProof.Entities.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Models
{
    public class Tests: IIdentity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public DifficultyLevel DifficultyLevel { get; set; }

        [Required]
        public bool Passed { get; set; }

        [Required]
        public int Score { get; set; }
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        public virtual JobApplications? JobApplication { get; set; }
        public virtual Users? User { get; set; }
        public virtual ICollection<TestAnswers> TestAnswers { get; set; }
    }
}
