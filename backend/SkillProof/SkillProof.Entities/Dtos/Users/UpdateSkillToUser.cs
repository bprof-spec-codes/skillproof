using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillProof.Entities.Dtos.Skill;
using SkillProof.Entities.Models;

namespace SkillProof.Entities.Dtos.Users
{
    public class UpdateSkillToUser
    {
        public List<SkillViewDto> Skills { get; set; } = new List<SkillViewDto>();
    }
}
