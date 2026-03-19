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
    public class JobbApplicationsConfigurations : IEntityTypeConfiguration<JobApplications>
    {
        public void Configure(EntityTypeBuilder<JobApplications> builder)
        {
            builder.HasKey(ja => ja.Id);
            builder.HasOne(ja => ja.Job)
                .WithMany(j => j.JobApplications)
                .HasForeignKey(ja => ja.JobId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(ja => ja.User)
                .WithMany(ja => ja.JobApplications)
                .HasForeignKey(ja => ja.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(ja => ja.Test)
               .WithOne(t => t.JobApplication)
               .HasForeignKey<JobApplications>(ja => ja.TestId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
