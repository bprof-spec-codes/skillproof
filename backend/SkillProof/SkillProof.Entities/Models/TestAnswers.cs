using SkillProof.Entities.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Models
{
    public class TestAnswers: IIdentity
    {
        [Key]
        public string Id { get; set; }

        [ForeignKey("Question")]
        public string QuestionId { get; set; }

        [ForeignKey("Test")]
        public string TestId { get; set; }

        public string FreeTextResponse { get; set; }

        [Required]
        public bool IsCorrect { get; set; }
        public string AiFeedback { get; set; }

        public virtual Questions Question { get; set; }
        public virtual Tests Test { get; set; }
    }
}
