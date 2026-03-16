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
    public class JobApplications: IIdentity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public JobApplicationStatus Status { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public virtual Jobs Job { get; set; }
        public virtual Users User { get; set; }
        public virtual Tests? Test { get; set; }
    }
}
