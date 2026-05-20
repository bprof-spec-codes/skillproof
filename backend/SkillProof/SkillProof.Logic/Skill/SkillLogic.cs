using Microsoft.EntityFrameworkCore;
using SkillProof.Data;
using SkillProof.Data.Repositorys;
using SkillProof.Entities.Dtos.Assesment;
using SkillProof.Entities.Dtos.Skill;
using SkillProof.Entities.Dtos.Tests;
using SkillProof.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Logic.Skill
{
    public class SkillLogic
    {

        private readonly IRepository<SkillModel> _skillRepository;
        private readonly SkillProofDbContext _ctx;

        public SkillLogic(IRepository<SkillModel> skillRepository, SkillProofDbContext _ctx)
        {
            this._skillRepository = skillRepository;
            this._ctx = _ctx;
        }

        public async Task<IEnumerable<ViewSkill>> GetAllSkillsAsync()
        {
            return await _skillRepository.GetAll()
                .Include(s => s.Assessments)
                .Select(s => new ViewSkill
                {
                    Id = s.Id,
                    Name = s.Name,
                    Assessments = s.Assessments.Select(a => new AssessmentViewDto
                    {
                        Id = a.Id,
                        Title = a.Title,
                        DifficultyLevel = a.DifficultyLevel
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<ViewSkill> CreateSkillAsync(SkillCreateDto model)
        {
            var skill = new SkillModel
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.Name
            };
            await _skillRepository.Create(skill);

            return new ViewSkill { Id = skill.Id, Name = skill.Name };
        }

        public async Task<ViewSkill> GetSkillByIdAsync(string id)
        {
            var skill = await _skillRepository.GetOne(id);
            return skill != null ? new ViewSkill { Id = skill.Id, Name = skill.Name } : null;
        }

        public async Task<CandidateAssessmentDto?> GetCandidateTestForSkill(string skillId, string assessmentId)
        {
            var skill = await _skillRepository.GetAll()
                .Include(s => s.Assessments)
                    .ThenInclude(a => a.Questions)
                        .ThenInclude(q => q.MultipleChoiceQuestion)
                .Include(s => s.Assessments)
                    .ThenInclude(a => a.Questions)
                        .ThenInclude(q => q.CodeCompletionQuestion)
                .Include(s => s.Assessments)
                    .ThenInclude(a => a.Questions)
                        .ThenInclude(q => q.FillInTheBlankQuestions)
                .Include(s => s.Assessments)
                    .ThenInclude(a => a.Questions)
                        .ThenInclude(q => q.TrueFalseQuestion)
                .FirstOrDefaultAsync(s => s.Id == skillId);

            if (skill == null || skill.Assessments.Count == 0)
            {
                return null;
            }

            var targetAssessment = skill.Assessments.FirstOrDefault(a => a.Id == assessmentId);
            if (targetAssessment == null)
            {
                return null;
            }

            var targetQuestions = targetAssessment.Questions.ToList();
            if (targetQuestions.Count == 0)
            {
                return null;
            }

            return new CandidateAssessmentDto
            {
                Id = targetAssessment.Id,
                Title = targetAssessment.Title,
                DifficultyLevel = targetAssessment.DifficultyLevel,
                Questions = targetQuestions.Select(q => new CandidateQuestionDto
                {
                    Id = q.Id,
                    Type = q.Type,
                    Title = q.Title,
                    QuestionText = q.QuestionText,
                    Language = q.Language,
                    Difficulty = q.Difficulty,

                    MultipleChoice = q.MultipleChoiceQuestion == null ? null : new CandidateMultipleChoicePayloadDto
                    {
                        Options = string.IsNullOrWhiteSpace(q.MultipleChoiceQuestion.Options)
                            ? new List<string>()
                            : System.Text.Json.JsonSerializer.Deserialize<List<string>>(q.MultipleChoiceQuestion.Options) ?? new List<string>(),
                        AllowMultipleSelection = q.MultipleChoiceQuestion.AllowMultipleSelection
                    },

                    CodeCompletion = q.CodeCompletionQuestion == null ? null : new CandidateCodeCompletionPayloadDto
                    {
                        CodeSnippet = q.CodeCompletionQuestion.CodeSnippet
                    },

                    FillInTheBlank = q.FillInTheBlankQuestions == null ? null : new CandidateFillInTheBlankPayloadDto(),

                    TrueFalse = q.TrueFalseQuestion == null ? null : new CandidateTrueFalsePayloadDto()
                }).ToList()
            };
        }

        public async Task DeleteSkill(string id)
        {
            var skill = await _skillRepository.GetOne(id);
            if (skill == null)
            {
                throw new Exception("Skill not found");
            }
            await _skillRepository.DeleteById(skill.Id);
        }
    }
}

