using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Dtos.Experience
{
    public class ExperienceCreateDto
    {
        public string JobTitle { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
