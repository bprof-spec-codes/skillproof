using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillProof.Entities.Models;

namespace SkillProof.Entities.Configurations;

public class AssessmentConfiguration : IEntityTypeConfiguration<Assessments>
{
    public void Configure(EntityTypeBuilder<Assessments> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.CreatedBy)
            .IsRequired();

        builder.HasMany(a => a.Questions)
            .WithMany(q => q.Assessments);

        builder.HasMany(a => a.Jobs)
            .WithMany(j => j.Assessments);
    }
}