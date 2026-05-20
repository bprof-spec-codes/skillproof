using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Dtos.Education
{
    public class EducationViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string School { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string FieldOfStudy { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsOngoing => EndDate == null;
        public string Description { get; set; } = string.Empty;
    }
}
