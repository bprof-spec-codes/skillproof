using SkillProof.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string Location { get; set; }

        public string Tags { get; set; } // Json array of string

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
