using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Models
{
    public class FillInTheBlankQuestions
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
        public string Answer { get; set; }
        // Ai feedback is in the testAnswers table
        public string? manualFeedback { get; set; }
        public virtual Questions Question { get; set; }
    }
}
