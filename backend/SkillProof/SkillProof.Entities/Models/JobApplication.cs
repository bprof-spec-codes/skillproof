using SkillProof.Entities.Enums;
using SkillProof.Entities.Helper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillProof.Entities.Models
{
    public class JobApplication: IIdentity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("Job")]
        public string JobId { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        [ForeignKey("Test")]
        public string? TestId { get; set; }

        [Required]
        public JobApplicationStatus Status { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public virtual Job Job { get; set; }
        public virtual Users User { get; set; }
        public virtual Tests? Test { get; set; }
    }
}
