using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Dtos.Tests
{
    public class UserTestReviewDto
    {
        public string TestAnswerId { get; set; }
        public double Score { get; set; }
        public string QuestionText { get; set; }
        public string UserResponse { get; set; }
        public bool Inspected { get; set; }

        //public string? AiFeedback { get; set; }

        public string UserId { get; set; }
    }
}
