using Microsoft.EntityFrameworkCore;
using SkillProof.Entities.Enums;
using SkillProof.Entities.Models;
using System;
using System.Linq;
using System.Text.Json;

namespace SkillProof.Data
{
    public static class DbInitializer
    {
        public static void Seed(DbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Set<Questions>().Any())
            {
                return;
            }

            var existingUser = context.Set<Users>().FirstOrDefault();
            var adminUserId = existingUser != null ? existingUser.Id : Guid.NewGuid().ToString();
            var testUserId = existingUser?.Id;

            var tfQuestionId = Guid.NewGuid().ToString();
            var tfQuestion = new Questions
            {
                Id = tfQuestionId,
                Type = QuestionType.TrueFalse,
                Language = "English",
                Difficulty = DifficultyLevel.Junior,
                Title = "C# OOP",
                QuestionText = "C# is an object-oriented programming language.",
                CreatedBy = adminUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var tfDetails = new TrueFalseQuestions
            {
                QuestionId = tfQuestionId,
                CorrectAnswer = true,
                Explanation = "C# supports encapsulation, inheritance, and polymorphism."
            };

            var mcQuestionId = Guid.NewGuid().ToString();
            var mcQuestion = new Questions
            {
                Id = mcQuestionId,
                Type = QuestionType.MultipleChoice,
                Language = "English",
                Difficulty = DifficultyLevel.Medior,
                Title = "C# Value Types",
                QuestionText = "Which of the following are value types in C#?",
                CreatedBy = adminUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var options = new[]
            {
                new { Id = "A", Text = "int" },
                new { Id = "B", Text = "string" },
                new { Id = "C", Text = "struct" },
                new { Id = "D", Text = "class" }
            };

            var correctAnswers = new[] { "A", "C" };

            var mcDetails = new MultipleChoiceQuestions
            {
                QuestionId = mcQuestionId,
                Options = JsonSerializer.Serialize(options),
                CorrectAnswerIds = JsonSerializer.Serialize(correctAnswers),
                AllowMultipleSelection = true
            };

            var testId = Guid.NewGuid().ToString();
            var test = new Tests
            {
                Id = testId,
                DifficultyLevel = DifficultyLevel.Medior,
                UserId = testUserId,
                Passed = true,
                Score = 85,
                CompletedAt = DateTime.UtcNow
            };

            var answer1 = new TestAnswers
            {
                Id = Guid.NewGuid().ToString(),
                TestId = testId,
                QuestionId = tfQuestionId,
                FreeTextResponse = "True",
                IsCorrect = true,
                AiFeedback = "Correct."
            };

            var answer2 = new TestAnswers
            {
                Id = Guid.NewGuid().ToString(),
                TestId = testId,
                QuestionId = mcQuestionId,
                FreeTextResponse = "A, C",
                IsCorrect = true,
                AiFeedback = "Accurate selection of value types."
            };

            context.Set<Questions>().AddRange(tfQuestion, mcQuestion);
            context.Set<TrueFalseQuestions>().Add(tfDetails);
            context.Set<MultipleChoiceQuestions>().Add(mcDetails);
            context.Set<Tests>().Add(test);
            context.Set<TestAnswers>().AddRange(answer1, answer2);

            context.SaveChanges();
        }
    }
}