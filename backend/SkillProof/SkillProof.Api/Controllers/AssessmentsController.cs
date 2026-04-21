using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SkillProof.Entities.Dtos.Assesment;
using SkillProof.Logic.Assesments;

namespace SkillProof.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentsController : ControllerBase
    {
        private readonly IAssessmentLogic _assessmentLogic;

        public AssessmentsController(IAssessmentLogic assessmentLogic)
        {
            _assessmentLogic = assessmentLogic;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssessment([FromBody] CreateAssessmentDto dto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User ID is missing from the token." });
            }

            var result = await _assessmentLogic.CreateAssessmentAsync(dto, userId);
            return CreatedAtAction(nameof(GetAssessmentById), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAssessments()
        {
            var results = await _assessmentLogic.GetAllAssessmentsAsync();
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAssessmentById(string id)
        {
            var result = await _assessmentLogic.GetAssessmentByIdAsync(id);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssessment(string id, [FromBody] UpdateAssessmentDto dto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User ID is missing from the token." });
            }

            var result = await _assessmentLogic.UpdateAssessmentAsync(id, dto, userId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssessment(string id)
        {
            await _assessmentLogic.DeleteAssessmentAsync(id);
            return NoContent();
        }
    }
}