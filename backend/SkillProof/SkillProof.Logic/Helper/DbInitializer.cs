using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillProof.Entities.Enums;
using SkillProof.Entities.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkillProof.Data
{
    public class QuestionSeedDto
    {
        public QuestionType Type { get; set; }
        public string? Language { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public string Title { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public TrueFalseSeedDto? TrueFalsePayload { get; set; }
        public TrueFalseSeedDto? TrueFalse { get; set; }
        public MultipleChoiceSeedDto? MultipleChoicePayload { get; set; }
        public MultipleChoiceSeedDto? MultipleChoice { get; set; }
        public FillInTheBlankSeedDto? FillInTheBlankPayload { get; set; }
        public FillInTheBlankSeedDto? FillInTheBlank { get; set; }
        public CodeCompletionSeedDto? CodeCompletionPayload { get; set; }
        public CodeCompletionSeedDto? CodeCompletion { get; set; }
    }

    public class TrueFalseSeedDto
    {
        public bool CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
    }

    public class MultipleChoiceSeedDto
    {
        public List<string> Options { get; set; } = new();
        public List<int>? CorrectAnswerIds { get; set; }
        public List<int>? CorrectOptionIndexes { get; set; }
        public bool AllowMultipleSelection { get; set; }
    }

    public class FillInTheBlankSeedDto
    {
        public string Answer { get; set; } = string.Empty;
        public string? ManualFeedback { get; set; }
    }

    public class CodeCompletionSeedDto
    {
        public string CodeSnippet { get; set; } = string.Empty;
        public List<string> AcceptedAnswers { get; set; } = new();
    }

    public static class DbInitializer
    {
        private const string SeedAssessmentTitle = "Full-Stack Developer Starter Kit";
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public static void Seed(DbContext context)
        {
            var existingSeed = context.Set<Assessments>()
                .Include(a => a.Questions)
                    .ThenInclude(q => q.MultipleChoiceQuestion)
                .Include(a => a.Questions)
                    .ThenInclude(q => q.CodeCompletionQuestion)
                .Include(a => a.Questions)
                    .ThenInclude(q => q.FillInTheBlankQuestions)
                .Include(a => a.Questions)
                    .ThenInclude(q => q.TrueFalseQuestion)
                .FirstOrDefault(a => a.Title == SeedAssessmentTitle);

            var adminRole = context.Set<IdentityRole>().FirstOrDefault(r => r.NormalizedName == "ADMIN");

            if (adminRole == null)
            {
                adminRole = new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                };
                context.Set<IdentityRole>().Add(adminRole);
            }

            var existingUser = context.Set<Users>().FirstOrDefault(u => u.NormalizedUserName == "SYSTEMADMIN");
            string adminUserId;

            if (existingUser != null)
            {
                adminUserId = existingUser.Id;
            }
            else
            {
                var newUser = new Users
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "systemadmin",
                    Email = "admin@system.local",
                    NormalizedUserName = "SYSTEMADMIN",
                    NormalizedEmail = "ADMIN@SYSTEM.LOCAL",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var passwordHasher = new PasswordHasher<Users>();
                newUser.PasswordHash = passwordHasher.HashPassword(newUser, "AdminPassword123!");

                context.Set<Users>().Add(newUser);
                adminUserId = newUser.Id;
            }

            var userRoleExists = context.Set<IdentityUserRole<string>>()
                .Any(ur => ur.UserId == adminUserId && ur.RoleId == adminRole.Id);

            if (!userRoleExists)
            {
                context.Set<IdentityUserRole<string>>().Add(new IdentityUserRole<string>
                {
                    UserId = adminUserId,
                    RoleId = adminRole.Id
                });
            }

            var seedFilePath = Path.Combine(AppContext.BaseDirectory, "seed-questions.json");

            if (!File.Exists(seedFilePath))
            {
                seedFilePath = Path.Combine(Directory.GetCurrentDirectory(), "seed-questions.json");

                if (!File.Exists(seedFilePath))
                {
                    throw new FileNotFoundException($"The seed JSON file was not found. Searched path: {seedFilePath}");
                }
            }

            var jsonContent = File.ReadAllText(seedFilePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new JsonStringEnumConverter());

            var seedQuestions = JsonSerializer.Deserialize<List<QuestionSeedDto>>(jsonContent, options);

            if (seedQuestions == null || !seedQuestions.Any())
            {
                throw new InvalidDataException("The seed JSON file was parsed but contains no valid questions.");
            }

            var seedAssessment = existingSeed ?? new Assessments
            {
                Id = Guid.NewGuid().ToString(),
                Title = SeedAssessmentTitle,
                Description = "Comprehensive test covering basic C# and .NET concepts.",
                DifficultyLevel = DifficultyLevel.Junior,
                CreatedBy = adminUserId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            seedAssessment.Title = SeedAssessmentTitle;
            seedAssessment.Description = "Comprehensive test covering basic C# and .NET concepts.";
            seedAssessment.DifficultyLevel = DifficultyLevel.Junior;
            seedAssessment.CreatedBy = adminUserId;
            seedAssessment.IsActive = true;

            if (existingSeed == null)
            {
                context.Set<Assessments>().Add(seedAssessment);
            }

            foreach (var sq in seedQuestions)
            {
                var question = seedAssessment.Questions
                    .FirstOrDefault(q => q.Title == sq.Title && q.Type == sq.Type);

                if (question == null)
                {
                    question = new Questions
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = sq.Type,
                        CreatedAt = DateTime.UtcNow
                    };

                    context.Set<Questions>().Add(question);
                    seedAssessment.Questions.Add(question);
                }

                question.Type = sq.Type;
                question.Language = NormalizeLanguage(sq.Language);
                question.Difficulty = sq.Difficulty;
                question.Title = sq.Title;
                question.QuestionText = sq.QuestionText;
                question.Tags = NormalizeTags(sq.Tags);
                question.CreatedBy = adminUserId;
                question.UpdatedAt = DateTime.UtcNow;
                question.IsActive = true;

                UpsertTypedPayload(context, question, sq);
            }

            context.SaveChanges();
        }

        private static void UpsertTypedPayload(DbContext context, Questions question, QuestionSeedDto seed)
        {
            switch (seed.Type)
            {
                case QuestionType.TrueFalse:
                {
                    var payload = seed.TrueFalse ?? seed.TrueFalsePayload
                        ?? throw new InvalidDataException($"Missing true/false payload for seed question '{seed.Title}'.");

                    var entity = question.TrueFalseQuestion
                        ?? context.Set<TrueFalseQuestions>().Find(question.Id)
                        ?? new TrueFalseQuestions { QuestionId = question.Id };

                    entity.CorrectAnswer = payload.CorrectAnswer;
                    entity.Explanation = payload.Explanation;

                    if (context.Entry(entity).State == EntityState.Detached)
                    {
                        context.Set<TrueFalseQuestions>().Add(entity);
                    }
                    break;
                }
                case QuestionType.MultipleChoice:
                {
                    var payload = seed.MultipleChoice ?? seed.MultipleChoicePayload
                        ?? throw new InvalidDataException($"Missing multiple-choice payload for seed question '{seed.Title}'.");

                    var entity = question.MultipleChoiceQuestion
                        ?? context.Set<MultipleChoiceQuestions>().Find(question.Id)
                        ?? new MultipleChoiceQuestions { QuestionId = question.Id };

                    entity.Options = JsonSerializer.Serialize(payload.Options, JsonOptions);
                    entity.CorrectAnswerIds = JsonSerializer.Serialize(payload.CorrectOptionIndexes ?? payload.CorrectAnswerIds ?? new List<int>(), JsonOptions);
                    entity.AllowMultipleSelection = payload.AllowMultipleSelection;

                    if (context.Entry(entity).State == EntityState.Detached)
                    {
                        context.Set<MultipleChoiceQuestions>().Add(entity);
                    }
                    break;
                }
                case QuestionType.FillInTheBlank:
                {
                    var payload = seed.FillInTheBlank ?? seed.FillInTheBlankPayload
                        ?? throw new InvalidDataException($"Missing fill-in-the-blank payload for seed question '{seed.Title}'.");

                    var entity = question.FillInTheBlankQuestions
                        ?? context.Set<FillInTheBlankQuestions>().Find(question.Id)
                        ?? new FillInTheBlankQuestions { QuestionId = question.Id };

                    entity.Answer = payload.Answer;
                    entity.manualFeedback = payload.ManualFeedback;

                    if (context.Entry(entity).State == EntityState.Detached)
                    {
                        context.Set<FillInTheBlankQuestions>().Add(entity);
                    }
                    break;
                }
                case QuestionType.CodeCompletion:
                {
                    var payload = seed.CodeCompletion ?? seed.CodeCompletionPayload
                        ?? throw new InvalidDataException($"Missing code-completion payload for seed question '{seed.Title}'.");

                    var entity = question.CodeCompletionQuestion
                        ?? context.Set<CodeCompletionQuestions>().Find(question.Id)
                        ?? new CodeCompletionQuestions { QuestionId = question.Id };

                    entity.CodeSnippet = payload.CodeSnippet;
                    entity.AcceptedAnswers = JsonSerializer.Serialize(payload.AcceptedAnswers, JsonOptions);

                    if (context.Entry(entity).State == EntityState.Detached)
                    {
                        context.Set<CodeCompletionQuestions>().Add(entity);
                    }
                    break;
                }
                default:
                    throw new InvalidDataException($"Unsupported seed question type '{seed.Type}' for '{seed.Title}'.");
            }
        }

        private static string NormalizeLanguage(string? language)
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                return "General";
            }

            var trimmed = language.Trim();
            return trimmed.Length > 20 ? trimmed[..20] : trimmed;
        }

        private static List<string> NormalizeTags(List<string>? tags)
        {
            return tags?
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Select(tag => tag.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();
        }
    }
}
