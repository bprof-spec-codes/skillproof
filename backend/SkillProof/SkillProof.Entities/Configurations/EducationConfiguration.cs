using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillProof.Entities.Models;

namespace SkillProof.Entities.Configurations
{
    public class EducationConfiguration : IEntityTypeConfiguration<Education>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Education> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.User)
                   .WithMany(u => u.Educations)
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
