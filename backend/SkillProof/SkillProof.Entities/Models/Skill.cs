using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Models
{
    public class Skill : IIdentity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ICollection<Users> Users { get; set; }
        public virtual ICollection<Assessments> Assesments { get; set; }

        public string? AuthenticationType => throw new NotImplementedException();

        public bool IsAuthenticated => throw new NotImplementedException();
    }
}
