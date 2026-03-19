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
    public class FillInTheBlankQuestionsConfigurations : IEntityTypeConfiguration<FillInTheBlankQuestions>
    {
        public void Configure(EntityTypeBuilder<FillInTheBlankQuestions> builder)
        {
            builder.HasKey(f => f.QuestionId);
            builder.HasOne(f => f.Question)
                .WithOne(q => q.FillInTheBlankQuestions)
                .HasForeignKey<FillInTheBlankQuestions>(f => f.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
