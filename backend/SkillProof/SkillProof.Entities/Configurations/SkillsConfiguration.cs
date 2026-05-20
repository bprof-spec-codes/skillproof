using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillProof.Entities.Models;

namespace SkillProof.Entities.Configurations
{
    public class SkillsConfiguration : IEntityTypeConfiguration<SkillModel>
    {
        public void Configure(EntityTypeBuilder<SkillModel> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Skill -> Assessments
            builder.HasMany(s => s.Assessments)
                .WithOne(a => a.Skill)
                .HasForeignKey(a => a.SkillId);

            // User <-> Skill many-to-many
            builder.HasMany(s => s.Users)
                .WithMany(u => u.Skills);
        }
    }
}
