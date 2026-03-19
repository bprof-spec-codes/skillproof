using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillProof.Entities.Models;

namespace SkillProof.Entities.Configurations
{
    public class CodeCompletionQuestionsConfigurations : IEntityTypeConfiguration<CodeCompletionQuestions>
    {
        public void Configure(EntityTypeBuilder<CodeCompletionQuestions> builder)
        {
            builder.HasKey(mcq => mcq.QuestionId);
            builder.HasOne(mcq => mcq.Question)
                .WithOne(q => q.CodeCompletionQuestion)
                .HasForeignKey<CodeCompletionQuestions>(mcq => mcq.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
