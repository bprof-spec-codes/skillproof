using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillProof.Entities.Configurations;
using SkillProof.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Data
{
    public class SkillProofDbContext : IdentityDbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<UserExperiences> UserExperiences { get; set; }
        public DbSet<Companies> Companies { get; set; }
        public DbSet<Jobs> Jobs { get; set; }
        public DbSet<Tests> Tests { get; set; }
        public DbSet<JobApplications> JobApplications { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<TestAnswers> TestAnswers { get; set; }
        public DbSet<MultipleChoiceQuestions> MultipleChoiceQuestion { get; set; }
        public DbSet<CodeCompletionQuestions> CodeCompletionQuestions { get; set; }
        public DbSet<FillInTheBlankQuestions> FillInTheBlankQuestions { get; set; }
        public DbSet<TrueFalseQuestions> TrueFalseQuestions { get; set; }
        public SkillProofDbContext(DbContextOptions<SkillProofDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new CodeCompletionQuestionsConfigurations());
            modelBuilder.ApplyConfiguration(new CompaniesConfigurations());
            modelBuilder.ApplyConfiguration(new JobbApplicationsConfigurations());
            modelBuilder.ApplyConfiguration(new JobsConfigurations());
            modelBuilder.ApplyConfiguration(new MultipleChoiceQuestionsConfigurations());
            modelBuilder.ApplyConfiguration(new QuestionsConfigurations());
            modelBuilder.ApplyConfiguration(new FillInTheBlankQuestionsConfigurations());
            modelBuilder.ApplyConfiguration(new TestAnswersConfigurations());
            modelBuilder.ApplyConfiguration(new TestsConfigurations());
            modelBuilder.ApplyConfiguration(new TrueFalseQuestionsConfigurations());
            modelBuilder.ApplyConfiguration(new UserExperiencesConfigurations());
            modelBuilder.ApplyConfiguration(new UsersConfigurations());
        }
    }
}
