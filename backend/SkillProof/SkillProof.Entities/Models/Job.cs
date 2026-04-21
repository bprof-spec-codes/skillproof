using SkillProof.Entities.Enums;
using SkillProof.Entities.Helper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillProof.Entities.Models
{
    public class Job: IIdentity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("Company")]
        public string CompanyId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Location { get; set; }

        public string Tags { get; set; } // Json array of strings

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public EmploymentType EmploymentType { get; set; }

        public virtual Companies Company { get; set; }
        public virtual ICollection<JobApplication>? JobApplications { get; set; }
        public virtual ICollection<Questions> Questions { get; set; } = new List<Questions>();
    }
}
