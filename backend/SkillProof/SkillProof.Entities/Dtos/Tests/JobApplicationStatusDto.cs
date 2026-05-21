using SkillProof.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Dtos.Tests
{
    public class JobApplicationStatusDto
    {
        public JobApplicationStatus jobApplicationStatus { get; set; }
        public string UserId { get; set; }
    }
}
