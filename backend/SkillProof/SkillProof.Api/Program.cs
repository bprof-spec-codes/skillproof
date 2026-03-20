using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillProof.Data;
using SkillProof.Entities.Models;
using SkillProof.Logic.Questions;

namespace SkillProof.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Frontend", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:4200", "https://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            builder.Services.AddDbContext<SkillProofDbContext>(options =>
                options.UseSqlServer(builder.Configuration["db:conn"]));

            builder.Services.AddIdentity<Users, IdentityRole>()
                .AddEntityFrameworkStores<SkillProofDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddScoped<IQuestionBankService, QuestionBankService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("Frontend");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
