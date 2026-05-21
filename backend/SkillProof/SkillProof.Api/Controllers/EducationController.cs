using Microsoft.AspNetCore.Mvc;
using SkillProof.Logic.Education;
using SkillProof.Entities.Dtos.Education;

namespace SkillProof.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EducationController : ControllerBase
    {
        private readonly IEducationLogic _educationLogic;
        public EducationController(IEducationLogic educationLogic)
        {
            _educationLogic = educationLogic;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetEducationsByUserId(string userId)
        {
            var educations = await _educationLogic.GetEducationsByUserIdAsync(userId);
            return Ok(educations);
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateEducation(string userId, [FromBody] EducationCreateDto entity)
        {
            try
            {
                var createdEducation = await _educationLogic.CreateEducationAsync(entity, userId);
                return Created("", createdEducation);
            }
            catch (Exception ex) when (ex.Message == "User not found")
            {
                return NotFound(new { message = "The specified user ID does not exist in the database." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while saving to the database.",
                    details = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpDelete("{userId}/{id}")]
        public async Task<IActionResult> DeleteEducation(string userId, string id)
        {
            await _educationLogic.DeleteEducationAsync(id, userId);
            return NoContent();
        }
    }
}
