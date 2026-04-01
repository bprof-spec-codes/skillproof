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
            try
            {
                var result = await _companyLogic.RegisterCompanyAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
            try
            {
                var companies = await _companyLogic.GetAllAsync();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyById(string id)
        {
            try
            {
                var company = await _companyLogic.GetByIdAsync(id);
                return Ok(company);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(string id, [FromBody] CompanyCreateDto dto)
        {
            try
            {
                var updatedCompany = await _companyLogic.UpdateAsync(id, dto);
                return Ok(updatedCompany);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(string id)
        {
            try
            {
                await _companyLogic.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}