using SkillProof.Entities.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Models
{
    public class Skill : IIdentity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }

        public virtual ICollection<Assessments> Assessments { get; set; } = new List<Assessments>();
        public virtual ICollection<Users> Users { get; set; } = new List<Users>();
    }
    
}
