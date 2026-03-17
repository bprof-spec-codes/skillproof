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
    public class JobsConfigurations : IEntityTypeConfiguration<Jobs>
    {
        public void Configure(EntityTypeBuilder<Jobs> builder)
        {
            builder.HasKey(j => j.Id);
            builder.HasOne(j => j.Company)
                   .WithMany(c => c.Jobs)
                   .HasForeignKey(j => j.CompanyId)
                   .OnDelete(DeleteBehavior.Cascade);
             builder.Property(j => j.Title)
                    .IsRequired()
                    .HasMaxLength(100);
             builder.Property(j => j.Description)
                    .IsRequired()
                    .HasMaxLength(1000);
             builder.Property(j => j.Location)
                    .IsRequired()
                    .HasMaxLength(100);
             builder.Property(j => j.Tags)
                    .HasMaxLength(500);
        }
    }
}
