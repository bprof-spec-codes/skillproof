using Microsoft.AspNetCore.Mvc;
using SkillProof.Entities.Dtos.Companies;
using SkillProof.Logic.Companies;

namespace SkillProof.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyLogic _companyLogic;

        public CompanyController(ICompanyLogic companyLogic)
        {
            _companyLogic = companyLogic;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterCompany([FromBody] CompanyCreateDto dto)
        {
                var result = await _companyLogic.RegisterCompanyAsync(dto);
                return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
                var companies = await _companyLogic.GetAllAsync();
                return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyById(string id)
        {
                var company = await _companyLogic.GetByIdAsync(id);
                return Ok(company);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(string id, [FromBody] CompanyCreateDto dto)
        {
                var updatedCompany = await _companyLogic.UpdateAsync(id, dto);
                return Ok(updatedCompany);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(string id)
        {
                await _companyLogic.DeleteAsync(id);
                return NoContent();
        }
    }
}