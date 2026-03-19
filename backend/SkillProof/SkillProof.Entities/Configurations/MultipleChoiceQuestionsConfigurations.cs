using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillProof.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Configurations
{
    public class MultipleChoiceQuestionsConfigurations : IEntityTypeConfiguration<MultipleChoiceQuestions>
    {
        public void Configure(EntityTypeBuilder<MultipleChoiceQuestions> builder)
        {
            builder.HasKey(mcq => mcq.QuestionId);
            builder.HasOne(mcq => mcq.Question)
                .WithOne(q => q.MultipleChoiceQuestion)
                .HasForeignKey<MultipleChoiceQuestions>(mcq => mcq.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
