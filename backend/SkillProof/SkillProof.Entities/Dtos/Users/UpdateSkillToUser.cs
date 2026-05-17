using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Dtos.Users
{
    public class UpdateSkillToUser
    {
        public string userId {  get; set; } = string.Empty;
        public string skillId { get; set; } = string.Empty;
    }
}
