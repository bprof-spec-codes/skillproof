using SkillProof.Entities.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Models
{
    public class TestAnswers: IIdentity
    {
        [Key]
        public string Id { get; set; }

        public string FreeTextResponse { get; set; }

        [Required]
        public bool IsCorrect { get; set; }
        public string AiFeedback { get; set; }

        public virtual Questions Question { get; set; }
        public virtual Tests Test { get; set; }
    }
}
