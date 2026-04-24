using SkillProof.Entities.Dtos.Assesment;
using SkillProof.Entities.Dtos.Questions;
using SkillProof.Entities.Enums;

namespace SkillProof.Entities.Dtos.Jobs
{
    public class JobViewDto
    {
        public string Id { get; set; }

        public string CompanyId { get; set; }

        public EmploymentType EmploymentType { get; set; }

        //public string CompanyName { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        
        public string ShortDescription { get; set; }

        public string Location { get; set; }

        public string Tags { get; set; } // Json array of string

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<AssessmentViewDto> Assessments { get; set; } = new List<AssessmentViewDto>();
        public List<string>? AssessmentIds { get; set; }
    }
}
