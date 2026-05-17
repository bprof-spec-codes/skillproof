using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SkillProof.Entities.Dtos;
using SkillProof.Entities.Dtos.Skill;
using SkillProof.Logic.Skill;


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
        public async Task<ICollection<ViewSkill>> GetAllSkills()
        {
            return await _skillLogic.GetSkills();
        }

        [HttpPost]
        public async Task AddSkillToTable(string skillName)
        {
            await _skillLogic.AddSkillToTable(skillName);
        }

    }
}
