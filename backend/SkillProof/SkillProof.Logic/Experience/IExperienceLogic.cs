using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillProof.Entities.Dtos.Experience;

namespace SkillProof.Logic.Experience
{
    public interface IExperienceLogic
    {
        Task<IEnumerable<ExperienceViewDto>> GetExperiencesByUserIdAsync(string userId);
        Task DeleteExperienceAsync(string id, string userId);
        Task<ExperienceCreateDto> CreateExperienceAsync(ExperienceCreateDto entity, string userId);
    }
}
