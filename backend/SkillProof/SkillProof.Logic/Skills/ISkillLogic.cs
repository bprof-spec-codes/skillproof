using SkillProof.Entities.Dtos.Jobs;
using SkillProof.Entities.Dtos.Skill;
using SkillProof.Entities.Dtos.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Logic.Skills
{
    public interface ISkillLogic
    {
        Task<IEnumerable<SkillViewDto>> GetAllSkillsAsync();
        Task<SkillViewDto> GetSkillByIdAsync(string id);
        Task<SkillViewDto> CreateSkillAsync(SkillCreateDto model);
        Task<CandidateAssessmentDto?> GetCandidateTestForSkill(string skillId);
    }
}
