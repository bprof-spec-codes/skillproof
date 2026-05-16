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
    public class SkillsConfiguration : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired();

            builder.HasMany(s => s.Assesments)
                .WithOne(s => s.Skill);

            builder.HasMany(u => u.Users)
                .WithMany(s => s.Skills);
        }
    }
}
