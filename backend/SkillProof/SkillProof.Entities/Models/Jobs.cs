using SkillProof.Entities.Enums;
using SkillProof.Entities.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Models
{
    public class Jobs: IIdentity
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

        [Column(TypeName = "nvarchar(max)")]
        public string Tags { get; set; } // Json array of strings

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public EmploymentType EmploymentType { get; set; }

        public virtual Companies Company { get; set; }
        public virtual ICollection<JobApplications>? JobApplications { get; set; }
    }
}
