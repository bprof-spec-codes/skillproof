using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SkillProof.Entities.Dtos;
using SkillProof.Entities.Dtos.Skill;
using SkillProof.Logic.Skill;
using Microsoft.AspNetCore.Authorization;


namespace SkillProof.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        public readonly SkillLogic _skillLogic;

        public SkillController(SkillLogic skillLogic)
        {
            this._skillLogic = skillLogic;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<ViewSkill>>> GetAllSkills()
        {
            var skills = await _skillLogic.GetAllSkillsAsync();
            return Ok(skills);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSkillById(string id)
        {
            var skill = await _skillLogic.GetSkillByIdAsync(id);
            return Ok(skill);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSkillAsync([FromBody] SkillCreateDto model)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User ID is missing from the token." });
            }

            var result = await _skillLogic.CreateSkillAsync(model);
            return CreatedAtAction(nameof(GetSkillById), new { id = result.Id }, result);

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSkill(string id)
        {
            try
            {
                await _skillLogic.DeleteSkill(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("{id}/test/{assessmentId}")]
        public async Task<IActionResult> GetTestForSkill(string id, string assessmentId)
        {
            var test = await _skillLogic.GetCandidateTestForSkill(id, assessmentId);
            if (test == null) return NotFound(new { message = "No test available for this skill/assessment level." });
            return Ok(test);
        }



    }
}
