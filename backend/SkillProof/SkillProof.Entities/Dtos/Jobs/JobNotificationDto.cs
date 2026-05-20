using SkillProof.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Dtos.Jobs
{
    public class JobNotificationDto
    {
        public string Id { get; set; }
        public string JobTitle { get; set; }
        public JobApplicationStatus Status { get; set; }

    }
}
