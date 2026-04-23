using SkillProof.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Dtos.Tests
{
    public class UserTestsDto
    {
        public DifficultyLevel DifficultyLevel { get; set; }
        public bool Passed { get; set; }
    }
}
