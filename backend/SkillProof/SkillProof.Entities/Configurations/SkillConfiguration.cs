using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillProof.Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SkillProof.Entities.Configurations
{
    public class SkillConfiguration : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasMany(s => s.Users)
                .WithMany(u => u.Skills)
                .UsingEntity(j => j.ToTable("UserSkills"));

            builder.HasMany(s => s.Assessments)
                .WithMany(a => a.Skills)
                .UsingEntity(j => j.ToTable("SkillAssessments"));

        }
    }
}
