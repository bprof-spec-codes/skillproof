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
        public string Language { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public string Title { get; set; }
        public string QuestionText { get; set; }
        public List<string> Tags { get; set; }
        public TrueFalseSeedDto? TrueFalsePayload { get; set; }
        public MultipleChoiceSeedDto? MultipleChoicePayload { get; set; }
    }

    public class TrueFalseSeedDto
    {
        public bool CorrectAnswer { get; set; }
        public string Explanation { get; set; }
    }

    public class MultipleChoiceSeedDto
    {
        public object Options { get; set; }
        public object CorrectAnswerIds { get; set; }
        public bool AllowMultipleSelection { get; set; }
    }

    public static class DbInitializer
    {
        public static void Seed(DbContext context)
        {
            var existingSeed = context.Set<Assessments>()
                .Include(a => a.Questions)
                .FirstOrDefault(a => a.Title == "Full-Stack Developer Starter Kit");

            if (existingSeed != null)
            {
                if (existingSeed.Questions != null && existingSeed.Questions.Any())
                {
                    return;
                }

                context.Set<Assessments>().Remove(existingSeed);
                context.SaveChanges();
            }

            var existingUser = context.Set<Users>().FirstOrDefault();
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

                context.Set<Users>().Add(newUser);
                adminUserId = newUser.Id;
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

            var generatedQuestions = new List<Questions>();

            foreach (var sq in seedQuestions)
            {
                var questionId = Guid.NewGuid().ToString();
                var question = new Questions
                {
                    Id = questionId,
                    Type = sq.Type,
                    Language = sq.Language,
                    Difficulty = sq.Difficulty,
                    Title = sq.Title,
                    QuestionText = sq.QuestionText,
                    Tags = sq.Tags ?? new List<string>(),
                    CreatedBy = adminUserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                generatedQuestions.Add(question);
                context.Set<Questions>().Add(question);

                if (sq.Type == QuestionType.TrueFalse && sq.TrueFalsePayload != null)
                {
                    context.Set<TrueFalseQuestions>().Add(new TrueFalseQuestions
                    {
                        QuestionId = questionId,
                        CorrectAnswer = sq.TrueFalsePayload.CorrectAnswer,
                        Explanation = sq.TrueFalsePayload.Explanation
                    });
                }
                else if (sq.Type == QuestionType.MultipleChoice && sq.MultipleChoicePayload != null)
                {
                    context.Set<MultipleChoiceQuestions>().Add(new MultipleChoiceQuestions
                    {
                        QuestionId = questionId,
                        Options = JsonSerializer.Serialize(sq.MultipleChoicePayload.Options),
                        CorrectAnswerIds = JsonSerializer.Serialize(sq.MultipleChoicePayload.CorrectAnswerIds),
                        AllowMultipleSelection = sq.MultipleChoicePayload.AllowMultipleSelection
                    });
                }
            }

            var defaultAssessment = new Assessments
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Full-Stack Developer Starter Kit",
                Description = "Comprehensive test covering basic C# and .NET concepts.",
                DifficultyLevel = DifficultyLevel.Junior,
                CreatedBy = adminUserId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Questions = generatedQuestions
            };

            context.Set<Assessments>().Add(defaultAssessment);
            context.SaveChanges();
        }
    }
}