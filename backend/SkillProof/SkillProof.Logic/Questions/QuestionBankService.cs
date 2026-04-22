using Microsoft.EntityFrameworkCore;
using SkillProof.Data;
using SkillProof.Entities.Dtos.Questions;
using SkillProof.Entities.Enums;
using SkillProof.Entities.Models;
using System.Text.Json;
using QuestionEntity = SkillProof.Entities.Models.Questions;

namespace SkillProof.Logic.Questions
{
    public class QuestionBankService : IQuestionBankService
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly SkillProofDbContext _dbContext;

        public QuestionBankService(SkillProofDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QuestionResponseDto> CreateAsync(CreateQuestionRequestDto request, CancellationToken cancellationToken = default)
        {
            ValidatePayload(request.Type, request.MultipleChoice, request.CodeCompletion, request.FillInTheBlank, request.TrueFalse);

            var entity = new QuestionEntity
            {
                Type = request.Type,
                Language = request.Language,
                Difficulty = request.Difficulty,
                Title = request.Title,
                QuestionText = request.QuestionText,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
                Tags = request.Tags
                
            };

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            _dbContext.Questions.Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            AddTypedQuestion(entity.Id, request.Type, request.MultipleChoice, request.CodeCompletion, request.FillInTheBlank, request.TrueFalse);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var created = await GetQuestionWithIncludes(entity.Id, cancellationToken);
            return MapToResponse(created!);
        }

        public async Task<IReadOnlyList<QuestionResponseDto>> GetAllAsync(QuestionListFilterDto filter, CancellationToken cancellationToken = default)
        {
            var query = BuildQuestionQuery();

            if (filter.Type.HasValue)
            {
                query = query.Where(q => q.Type == filter.Type.Value);
            }

            if (filter.Difficulty.HasValue)
            {
                query = query.Where(q => q.Difficulty == filter.Difficulty.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Language))
            {
                query = query.Where(q => q.Language == filter.Language);
            }

            if (filter.IsActive.HasValue)
            {
                query = query.Where(q => q.IsActive == filter.IsActive.Value);
            }

            var entities = await query
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync(cancellationToken);

            return entities.Select(MapToResponse).ToList();
        }

        public async Task<QuestionResponseDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var entity = await GetQuestionWithIncludes(id, cancellationToken);
            return entity == null ? null : MapToResponse(entity);
        }

        public async Task<QuestionResponseDto?> UpdateAsync(string id, UpdateQuestionRequestDto request, CancellationToken cancellationToken = default)
        {
            var entity = await GetQuestionWithIncludes(id, cancellationToken);
            if (entity == null)
            {
                return null;
            }

            ValidatePayload(entity.Type, request.MultipleChoice, request.CodeCompletion, request.FillInTheBlank, request.TrueFalse);

            entity.Language = request.Language;
            entity.Difficulty = request.Difficulty;
            entity.Title = request.Title;
            entity.QuestionText = request.QuestionText;
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            UpdateTypedQuestion(entity, request.MultipleChoice, request.CodeCompletion, request.FillInTheBlank, request.TrueFalse);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return MapToResponse(entity);
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbContext.Questions.FindAsync([id], cancellationToken);
            if (entity == null)
            {
                return false;
            }

            _dbContext.Questions.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        private IQueryable<QuestionEntity> BuildQuestionQuery()
        {
            return _dbContext.Questions
                .AsNoTracking()
                .Include(q => q.MultipleChoiceQuestion)
                .Include(q => q.CodeCompletionQuestion)
                .Include(q => q.FillInTheBlankQuestions)
                .Include(q => q.TrueFalseQuestion);
        }

        private Task<QuestionEntity?> GetQuestionWithIncludes(string id, CancellationToken cancellationToken)
        {
            return _dbContext.Questions
                .Include(q => q.MultipleChoiceQuestion)
                .Include(q => q.CodeCompletionQuestion)
                .Include(q => q.FillInTheBlankQuestions)
                .Include(q => q.TrueFalseQuestion)
                .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
        }

        private void AddTypedQuestion(
            string questionId,
            QuestionType type,
            MultipleChoiceQuestionPayloadDto? multipleChoice,
            CodeCompletionQuestionPayloadDto? codeCompletion,
            FillInTheBlankQuestionPayloadDto? fillInTheBlank,
            TrueFalseQuestionPayloadDto? trueFalse)
        {
            switch (type)
            {
                case QuestionType.MultipleChoice:
                    _dbContext.MultipleChoiceQuestion.Add(new MultipleChoiceQuestions
                    {
                        QuestionId = questionId,
                        Options = JsonSerializer.Serialize(multipleChoice!.Options, JsonOptions),
                        CorrectAnswerIds = JsonSerializer.Serialize(multipleChoice.CorrectOptionIndexes, JsonOptions),
                        AllowMultipleSelection = multipleChoice.AllowMultipleSelection
                    });
                    break;
                case QuestionType.CodeCompletion:
                    _dbContext.CodeCompletionQuestions.Add(new CodeCompletionQuestions
                    {
                        QuestionId = questionId,
                        CodeSnippet = codeCompletion!.CodeSnippet,
                        AcceptedAnswers = JsonSerializer.Serialize(codeCompletion.AcceptedAnswers, JsonOptions)
                    });
                    break;
                case QuestionType.FillInTheBlank:
                    _dbContext.FillInTheBlankQuestions.Add(new FillInTheBlankQuestions
                    {
                        QuestionId = questionId,
                        Answer = fillInTheBlank!.Answer,
                        manualFeedback = fillInTheBlank.ManualFeedback
                    });
                    break;
                case QuestionType.TrueFalse:
                    _dbContext.TrueFalseQuestions.Add(new TrueFalseQuestions
                    {
                        QuestionId = questionId,
                        CorrectAnswer = trueFalse!.CorrectAnswer,
                        Explanation = trueFalse.Explanation
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), $"Unsupported question type: {type}");
            }
        }

        private static void ValidatePayload(
            QuestionType type,
            MultipleChoiceQuestionPayloadDto? multipleChoice,
            CodeCompletionQuestionPayloadDto? codeCompletion,
            FillInTheBlankQuestionPayloadDto? fillInTheBlank,
            TrueFalseQuestionPayloadDto? trueFalse)
        {
            var hasMultipleChoice = multipleChoice != null;
            var hasCodeCompletion = codeCompletion != null;
            var hasFillInTheBlank = fillInTheBlank != null;
            var hasTrueFalse = trueFalse != null;

            var payloadCount = (hasMultipleChoice ? 1 : 0)
                + (hasCodeCompletion ? 1 : 0)
                + (hasFillInTheBlank ? 1 : 0)
                + (hasTrueFalse ? 1 : 0);

            if (payloadCount != 1)
            {
                throw new ArgumentException("Exactly one type-specific payload must be provided.");
            }

            var expectedMatches = (type == QuestionType.MultipleChoice && hasMultipleChoice)
                || (type == QuestionType.CodeCompletion && hasCodeCompletion)
                || (type == QuestionType.FillInTheBlank && hasFillInTheBlank)
                || (type == QuestionType.TrueFalse && hasTrueFalse);

            if (!expectedMatches)
            {
                throw new ArgumentException("The provided payload does not match the selected question type.");
            }
        }

        private void UpdateTypedQuestion(
            QuestionEntity entity,
            MultipleChoiceQuestionPayloadDto? multipleChoice,
            CodeCompletionQuestionPayloadDto? codeCompletion,
            FillInTheBlankQuestionPayloadDto? fillInTheBlank,
            TrueFalseQuestionPayloadDto? trueFalse)
        {
            switch (entity.Type)
            {
                case QuestionType.MultipleChoice:
                    entity.MultipleChoiceQuestion!.Options = JsonSerializer.Serialize(multipleChoice!.Options, JsonOptions);
                    entity.MultipleChoiceQuestion.CorrectAnswerIds = JsonSerializer.Serialize(multipleChoice.CorrectOptionIndexes, JsonOptions);
                    entity.MultipleChoiceQuestion.AllowMultipleSelection = multipleChoice.AllowMultipleSelection;
                    break;
                case QuestionType.CodeCompletion:
                    entity.CodeCompletionQuestion!.CodeSnippet = codeCompletion!.CodeSnippet;
                    entity.CodeCompletionQuestion.AcceptedAnswers = JsonSerializer.Serialize(codeCompletion.AcceptedAnswers, JsonOptions);
                    break;
                case QuestionType.FillInTheBlank:
                    entity.FillInTheBlankQuestions!.Answer = fillInTheBlank!.Answer;
                    entity.FillInTheBlankQuestions.manualFeedback = fillInTheBlank.ManualFeedback;
                    break;
                case QuestionType.TrueFalse:
                    entity.TrueFalseQuestion!.CorrectAnswer = trueFalse!.CorrectAnswer;
                    entity.TrueFalseQuestion.Explanation = trueFalse.Explanation;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(entity.Type), $"Unsupported question type: {entity.Type}");
            }
        }

        private static QuestionResponseDto MapToResponse(QuestionEntity question)
        {
            return new QuestionResponseDto
            {
                Id = question.Id,
                Type = question.Type,
                Language = question.Language,
                Difficulty = question.Difficulty,
                Title = question.Title,
                QuestionText = question.QuestionText,
                CreatedBy = question.CreatedBy,
                IsActive = question.IsActive,
                CreatedAt = question.CreatedAt,
                UpdatedAt = question.UpdatedAt,
                MultipleChoice = question.MultipleChoiceQuestion == null
                    ? null
                    : new MultipleChoiceQuestionPayloadDto
                    {
                        Options = TryDeserialize<List<string>>(question.MultipleChoiceQuestion.Options) ?? new List<string>(),
                        CorrectOptionIndexes = TryDeserialize<List<int>>(question.MultipleChoiceQuestion.CorrectAnswerIds) ?? new List<int>(),
                        AllowMultipleSelection = question.MultipleChoiceQuestion.AllowMultipleSelection
                    },
                CodeCompletion = question.CodeCompletionQuestion == null
                    ? null
                    : new CodeCompletionQuestionPayloadDto
                    {
                        CodeSnippet = question.CodeCompletionQuestion.CodeSnippet,
                        AcceptedAnswers = TryDeserialize<List<string>>(question.CodeCompletionQuestion.AcceptedAnswers) ?? new List<string>()
                    },
                FillInTheBlank = question.FillInTheBlankQuestions == null
                    ? null
                    : new FillInTheBlankQuestionPayloadDto
                    {
                        Answer = question.FillInTheBlankQuestions.Answer,
                        ManualFeedback = question.FillInTheBlankQuestions.manualFeedback
                    },
                TrueFalse = question.TrueFalseQuestion == null
                    ? null
                    : new TrueFalseQuestionPayloadDto
                    {
                        CorrectAnswer = question.TrueFalseQuestion.CorrectAnswer,
                        Explanation = question.TrueFalseQuestion.Explanation
                    }
            };
        }

        private static T? TryDeserialize<T>(string raw)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(raw, JsonOptions);
            }
            catch (JsonException)
            {
                return default;
            }
        }
    }
}
