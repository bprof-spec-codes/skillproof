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
    public class MultipleChoiceQuestions: IIdentity
    {
        [Key]
        [ForeignKey("Question")]
        public string QuestionId { get; set; }

        [NotMapped]
        public string Id
        {
            get => QuestionId;
            set => QuestionId = value;
        }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Options { get; set; } // JSON array of AnswerOption

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CorrectAnswerIds { get; set; } // JSON array of strings

        public bool AllowMultipleSelection { get; set; } = false;

        public virtual Questions Question { get; set; }
    }
}
