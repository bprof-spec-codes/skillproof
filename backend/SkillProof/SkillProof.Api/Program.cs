using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SkillProof.Data;
using SkillProof.Data.Repositorys;
using SkillProof.Entities.Helper;
using SkillProof.Entities.Models;
using SkillProof.Logic.Helper;
using SkillProof.Logic.Jobs;
using SkillProof.Logic.Questions;
using SkillProof.Logic.User;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using SkillProof.Logic.Assesments;
using SkillProof.Logic.Assessments;

namespace SkillProof.Api
{
    public class Program
    {

        public static void Main(string[] args)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var builder = WebApplication.CreateBuilder(args);

            //Connection string from json file
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            //JWT configuration from json file
            //The defined properties are now in a class, inside the properties of it
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

            var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

            var jwtIssuer = jwtSettings!.Issuer;
            var jwtKey = jwtSettings!.Key;

            //Cors configuration from json file
            var frontendUrl = builder.Configuration["settings:frontend"];

            // Add services to the container.
            
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Frontend", policy =>
                {
                    policy
                        .WithOrigins(frontendUrl ?? "http://localhost:4200", "https://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            builder.Services.AddTransient(typeof(Repository<>));
            builder.Services.AddScoped<IMarkdownService, MarkdownService>();

            builder.Services.AddCors(option =>
            {
                option.AddPolicy("AllowAngularApp", policy =>
                {
                    policy.WithOrigins(frontendUrl!)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                });
            });
            
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });

            //Auth

            builder.Services.AddIdentity<Users, IdentityRole>(
                option =>
                {
                    option.Password.RequireDigit = false;
                    option.Password.RequiredLength = 8;
                    option.Password.RequireNonAlphanumeric = false;
                    option.Password.RequireUppercase = false;
                    option.Password.RequireLowercase = false;
                })
                .AddEntityFrameworkStores<SkillProofDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false; //fejlesztéshez jobb ha nincs bekapcsolva
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero
                };
            }); ;

            //DbCtx

            builder.Services.AddDbContext<SkillProofDbContext>(options =>
                options.UseSqlServer(builder.Configuration["db:conn"]));

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "SkillProof API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
  #region DI
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IQuestionBankService, QuestionBankService>();
            builder.Services.AddScoped<IJobLogic, JobLogic>();
            builder.Services.AddScoped<IUserLogic, UserLogic>();
            builder.Services.AddScoped<IAssessmentLogic, AssessmentLogic>();
#endregion
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseCors("AllowAngularApp");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseStaticFiles();

            app.MapControllers();
                
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<SkillProofDbContext>();
                    DbInitializer.Seed(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database seeding failed: {ex.Message}");
                }
            }
            
            app.Run();
        }
    }
}
