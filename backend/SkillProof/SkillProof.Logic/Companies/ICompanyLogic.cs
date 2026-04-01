using SkillProof.Entities.Dtos.Companies;

namespace SkillProof.Logic.Companies;

public interface ICompanyLogic
{
    Task<CompanyViewDto> RegisterCompanyAsync(CompanyCreateDto dto);
    Task<IEnumerable<CompanyViewDto>> GetAllAsync();
    Task<CompanyViewDto> GetByIdAsync(string id);
    Task<CompanyViewDto> UpdateAsync(string id, CompanyCreateDto dto);
    Task DeleteAsync(string id);
}