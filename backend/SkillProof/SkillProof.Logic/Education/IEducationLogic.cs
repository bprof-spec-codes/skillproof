using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillProof.Entities.Dtos.Education;

namespace SkillProof.Logic.Education
{
    public interface IEducationLogic
    {
        Task<IEnumerable<EducationViewDto>> GetEducationsByUserIdAsync(string userId);
        Task DeleteEducationAsync(string id, string userId);
        Task<EducationCreateDto> CreateEducationAsync(EducationCreateDto entity, string userId);
    }
}
