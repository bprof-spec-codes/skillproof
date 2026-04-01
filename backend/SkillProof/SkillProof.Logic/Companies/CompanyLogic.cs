using Microsoft.EntityFrameworkCore;
using SkillProof.Data.Repositorys;
using SkillProof.Entities.Dtos.Companies;

namespace SkillProof.Logic.Companies;

public class CompanyLogic : ICompanyLogic
{
    private readonly IRepository<SkillProof.Entities.Models.Companies> _companyRepository;

        public CompanyLogic(IRepository<SkillProof.Entities.Models.Companies> companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<CompanyViewDto> RegisterCompanyAsync(CompanyCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Company name is required.");
            }

            var newCompany = new SkillProof.Entities.Models.Companies
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Description = dto.Description,
                Website = dto.Website,
                CreatedAt = DateTime.UtcNow
            };

            await _companyRepository.Create(newCompany);

            return new CompanyViewDto
            {
                Id = newCompany.Id,
                Name = newCompany.Name,
                Description = newCompany.Description,
                Website = newCompany.Website,
                CreatedAt = newCompany.CreatedAt
            };
        }

        public async Task<IEnumerable<CompanyViewDto>> GetAllAsync()
        {
            return await _companyRepository.GetAll().Select(c => new CompanyViewDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Website = c.Website,
                CreatedAt = c.CreatedAt
            }).ToListAsync();
        }

        public async Task<CompanyViewDto> GetByIdAsync(string id)
        {
            var company = await _companyRepository.GetOne(id);
            if (company == null)
            {
                throw new KeyNotFoundException("Company not found.");
            }

            return new CompanyViewDto
            {
                Id = company.Id,
                Name = company.Name,
                Description = company.Description,
                Website = company.Website,
                CreatedAt = company.CreatedAt
            };
        }

        public async Task<CompanyViewDto> UpdateAsync(string id, CompanyCreateDto dto)
        {
            var company = await _companyRepository.GetOne(id);
            if (company == null)
            {
                throw new KeyNotFoundException("Company not found.");
            }

            company.Name = dto.Name;
            company.Description = dto.Description;
            company.Website = dto.Website;

            await _companyRepository.Update(company);

            return new CompanyViewDto
            {
                Id = company.Id,
                Name = company.Name,
                Description = company.Description,
                Website = company.Website,
                CreatedAt = company.CreatedAt
            };
        }

        public async Task DeleteAsync(string id)
        {
            var company = await _companyRepository.GetOne(id);
            if (company == null)
            {
                throw new KeyNotFoundException("Company not found.");
            }

            await _companyRepository.DeleteById(id);
        }
    }
