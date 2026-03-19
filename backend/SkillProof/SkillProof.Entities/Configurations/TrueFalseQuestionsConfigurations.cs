using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillProof.Entities.Models;

namespace SkillProof.Entities.Configurations
{
    public class TrueFalseQuestionsConfigurations : IEntityTypeConfiguration<TrueFalseQuestions>
    {
        public void Configure(EntityTypeBuilder<TrueFalseQuestions> builder)
        {
            builder.HasKey(tf => tf.QuestionId);
            builder.HasOne(tf => tf.Question)
                .WithOne(q => q.TrueFalseQuestion)
                .HasForeignKey<TrueFalseQuestions>(tf => tf.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
