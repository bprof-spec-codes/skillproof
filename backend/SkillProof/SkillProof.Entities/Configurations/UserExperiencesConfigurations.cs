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
    public class UserExperiencesConfigurations : IEntityTypeConfiguration<UserExperiences>
    {
        public void Configure(EntityTypeBuilder<UserExperiences> builder)
        {
            builder.HasKey(ue => ue.Id);
            builder.Property(ue => ue.JobTitle)
                .HasMaxLength(100);
            builder.Property(ue => ue.CompanyName)
                .HasMaxLength(100);
            builder.HasOne(ue => ue.User)
                .WithMany(u => u.UserExperiences)
                .HasForeignKey(ue => ue.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
