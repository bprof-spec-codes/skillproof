using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using SkillProof.Entities;
using SkillProof.Entities.Helper;

namespace SkillProof.Entities.Models
{
    public class Skill: Helper.IIdentity
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // Egy skillhez több assessment
        public virtual ICollection<Assessments> Assessments { get; set; }
            = new List<Assessments>();

        // Több usernek lehet ugyanaz a skillje
        public virtual ICollection<Users> Users { get; set; }
            = new List<Users>();

        public Skill()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
