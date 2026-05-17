using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SkillProof.Entities.Dtos.Skill;
using SkillProof.Logic.Skills;
using System.Security.Claims;


namespace SkillProof.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : Controller
    {
        private readonly ISkillLogic _skillLogic;

        public SkillController(ISkillLogic skillLogic)
        {
            _skillLogic = skillLogic;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
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

        [HttpGet("{id}/test")]
        public async Task<IActionResult> GetTestForSkill(string id)
        {
            var test = await _skillLogic.GetCandidateTestForSkill(id);
            if (test == null) return NotFound(new { message = "No test available for this skill." });
            return Ok(test);
        }
    }
}
