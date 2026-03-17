using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
        public DbSet<Users> Users;
        public DbSet<UserExperiences> UserExperiences;
        public DbSet<Companies> Companies;
        public DbSet<Jobs> Jobs;
        public DbSet<Tests> Tests;
        public DbSet<JobApplications> JobApplications;
        public DbSet<Questions> Questions;
        public DbSet<TestAnswers> TestAnswers;
        public DbSet<MultipleChoiceQuestions> MultipleChoiceQuestions;
        public DbSet<CodeCompletionQuestions> CodeCompletionQuestions;
        public SkillProofDbContext(DbContextOptions<SkillProofDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
