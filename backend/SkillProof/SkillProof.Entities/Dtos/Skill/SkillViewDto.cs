using SkillProof.Entities.Dtos.Assesment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Dtos.Skill
{
    public class SkillViewDto
    {
        public string Id { get; set; }
        public string Name { get; set;  }
        public List<AssessmentViewDto> Assessments { get; set; } = new List<AssessmentViewDto>();
    }
}
