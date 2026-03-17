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
    public class TestsConfigurations : IEntityTypeConfiguration<Tests>
    {
        public void Configure(EntityTypeBuilder<Tests> builder)
        {
            builder.HasKey(t => t.Id);
            builder.HasOne(t => t.User)
                .WithMany(u => u.Tests)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
