using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillProof.Entities.Models;

namespace SkillProof.Entities.Configurations
{
    public class UsersConfigurations : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(50);
            builder.Property(u => u.Headline).HasMaxLength(100);
            builder.Property(u => u.Bio).HasMaxLength(500);
            builder.HasMany(u => u.SavedJobs)
                   .WithMany()
                   .UsingEntity(j => j.ToTable("UserSavedJobs"));
        }
    }
}
