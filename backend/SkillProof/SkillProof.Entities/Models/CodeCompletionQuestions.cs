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
    public class CodeCompletionQuestions: IIdentity
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

        [Column(TypeName = "text")]
        public string CodeSnippet { get; set; }

        [Column(TypeName = "text")]
        public string AcceptedAnswers { get; set; } // JSON array of strings


        public virtual Questions Question { get; set; }
    }
}
