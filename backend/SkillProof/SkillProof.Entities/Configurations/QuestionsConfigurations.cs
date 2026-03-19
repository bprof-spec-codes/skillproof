using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using SkillProof.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Configurations
{
    public class QuestionsConfigurations : IEntityTypeConfiguration<Questions>
    {
        public void Configure(EntityTypeBuilder<Questions> builder)
        {
            builder.HasKey(q => q.Id);
            builder.HasMany(q => q.TestAnswers)
                   .WithOne(ta => ta.Question)
                   .HasForeignKey(ta => ta.QuestionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
