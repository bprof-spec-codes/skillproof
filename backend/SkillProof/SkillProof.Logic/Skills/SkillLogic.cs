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

namespace SkillProof.Logic.Skills
{
    public class SkillLogic : ISkillLogic
    {
        private readonly IRepository<Skill> _skillRepository;

        public SkillLogic(IRepository<Skill> skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public async Task<IEnumerable<SkillViewDto>> GetAllSkillsAsync()
        {
            return await _skillRepository.GetAll()
                .Include(s => s.Assessments)
                .Select(s => new SkillViewDto
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

        public async Task<SkillViewDto> GetSkillByIdAsync(string id)
        {
            var skill = await _skillRepository.GetOne(id);
            return skill != null ? new SkillViewDto { Id = skill.Id, Name = skill.Name } : null;
        }

        public async Task<SkillViewDto> CreateSkillAsync(SkillCreateDto model)
        {
            var skill = new Skill
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.Name
            };
            await _skillRepository.Create(skill);
            
            return new SkillViewDto { Id = skill.Id, Name = skill.Name };
        }

        public async Task<CandidateAssessmentDto?> GetCandidateTestForSkill(string skillId)
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

            var firstAssessment = skill.Assessments.FirstOrDefault();
            if (firstAssessment == null)
            {
                return null;
            }

            var allQuestions = skill.Assessments.SelectMany(a => a.Questions).ToList();
            if (allQuestions.Count == 0)
            {
                return null;
            }

            return new CandidateAssessmentDto
            {
                Id = firstAssessment.Id,
                Title = firstAssessment.Title,
                DifficultyLevel = firstAssessment.DifficultyLevel,
                Questions = allQuestions.Select(q => new CandidateQuestionDto
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
    }
}
