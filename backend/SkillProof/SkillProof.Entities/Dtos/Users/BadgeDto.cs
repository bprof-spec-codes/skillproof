using SkillProof.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Dtos.Users
{
    public class BadgeDto
    {
        public string SourceName { get; set; } = string.Empty;
        public DifficultyLevel DifficultyLevel { get; set; }
        public DateTime IssuedAt { get; set; }
    }
}
