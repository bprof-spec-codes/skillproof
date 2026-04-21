using Microsoft.EntityFrameworkCore;
using SkillProof.Entities.Enums;
using SkillProof.Entities.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            context.Database.EnsureCreated();

            if (context.Set<Questions>().Any() || context.Set<Assessments>().Any())
            {
                return;
            }

            var existingUser = context.Set<Users>().FirstOrDefault();
            var adminUserId = existingUser != null ? existingUser.Id : Guid.NewGuid().ToString();

            // Debugging tipp: kiírathatod az elérési utat a konzolra, ha továbbra sem találja
            var seedFilePath = Path.Combine(AppContext.BaseDirectory, "seed-questions.json");

            if (!File.Exists(seedFilePath))
            {
                // Ha nem találja a bin-ben, megpróbáljuk a gyökérben (fejlesztői környezet)
                seedFilePath = Path.Combine(Directory.GetCurrentDirectory(), "seed-questions.json");

                if (!File.Exists(seedFilePath))
                {
                    throw new FileNotFoundException($"The seed JSON file was not found. Looked in: {seedFilePath}");
                }
            }

            var jsonContent = File.ReadAllText(seedFilePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new JsonStringEnumConverter());

            var seedQuestions = JsonSerializer.Deserialize<List<QuestionSeedDto>>(jsonContent, options);

            if (seedQuestions == null || !seedQuestions.Any())
            {
                return;
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