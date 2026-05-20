using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillProof.Logic.Experience;
using SkillProof.Entities.Dtos.Experience;
namespace SkillProof.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExperienceController : ControllerBase
    {
        private readonly IExperienceLogic _experienceLogic;

        public ExperienceController(IExperienceLogic experienceLogic)
        {
            _experienceLogic = experienceLogic;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetExperiencesByUserId(string userId)
        {
            var experiences = await _experienceLogic.GetExperiencesByUserIdAsync(userId);
            return Ok(experiences);
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateExperience(string userId, [FromBody] ExperienceCreateDto entity)
        {
            var createdExperience = await _experienceLogic.CreateExperienceAsync(entity, userId);
            return CreatedAtAction(nameof(GetExperiencesByUserId), new { userId }, createdExperience);
        }

        [HttpDelete("{userId}/{id}")]
        public async Task<IActionResult> DeleteExperience(string userId, string id)
        {
            await _experienceLogic.DeleteExperienceAsync(id, userId);
            return NoContent();
        }
    }
}
