using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillProof.Entities.Helper;

namespace SkillProof.Entities.Models
{
    public class Education : IIdentity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("User")]
        public string UserId { get; set; }

        [Required]
        public string School { get; set; }

        public string Degree { get; set; }

        public string FieldOfStudy { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsOngoing => EndDate == null;

        public string Description { get; set; }

        public virtual Users User { get; set; }
    }
}
