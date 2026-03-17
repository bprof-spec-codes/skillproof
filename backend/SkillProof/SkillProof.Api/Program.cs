using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillProof.Data;
using SkillProof.Entities.Models;

namespace SkillProof.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<SkillProofDbContext>(options =>
                options.UseSqlServer(builder.Configuration["db:conn"]));

            builder.Services.AddIdentity<Users, IdentityRole>()
                .AddEntityFrameworkStores<SkillProofDbContext>()
                .AddDefaultTokenProviders();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
