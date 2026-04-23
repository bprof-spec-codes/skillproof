using SkillProof.Entities.Enums;
using SkillProof.Entities.Helper;
using SkillProof.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Models
{
    public class Questions: IIdentity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public QuestionType Type { get; set; }

        [Required]
        [StringLength(20)]
        public string Language { get; set; }

        [Required]
        public DifficultyLevel Difficulty { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        public string QuestionText { get; set; }

        [Required]
        public List<string> Tags { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(450)]
        public string CreatedBy { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<TestAnswers> TestAnswers { get; set; }
        public virtual MultipleChoiceQuestions? MultipleChoiceQuestion { get; set; }
        public virtual FillInTheBlankQuestions? FillInTheBlankQuestions { get; set; }
        public virtual CodeCompletionQuestions? CodeCompletionQuestion { get; set; }
        public virtual TrueFalseQuestions? TrueFalseQuestion { get; set; }
        public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
        public virtual ICollection<Assessments> Assessments { get; set; } = new List<Assessments>();
    }
}
