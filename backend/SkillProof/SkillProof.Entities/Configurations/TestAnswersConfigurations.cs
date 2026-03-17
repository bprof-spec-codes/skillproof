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
    public class TestAnswersConfigurations : IEntityTypeConfiguration<TestAnswers>
    {
        public void Configure(EntityTypeBuilder<TestAnswers> builder)
        {
            builder.HasKey(ta => ta.Id);
            builder.Property(ta => ta.FreeTextResponse)
                .HasMaxLength(500);
            builder.Property(ta => ta.AiFeedback)
                .HasMaxLength(500);
            builder.HasOne(ta => ta.Question)
                .WithMany(q => q.TestAnswers)
                .HasForeignKey(ta => ta.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(ta => ta.Test)
                .WithMany(t => t.TestAnswers)
                .HasForeignKey(ta => ta.TestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
