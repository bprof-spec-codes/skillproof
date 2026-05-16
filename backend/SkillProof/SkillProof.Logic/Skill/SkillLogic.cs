using Microsoft.EntityFrameworkCore;
using SkillProof.Data;
using SkillProof.Data.Repositorys;
using SkillProof.Entities.Dtos.Skill;
using SkillProof.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Logic.Skill
{
    public class SkillLogic
    {

        private readonly IRepository<SkillProof.Entities.Models.Skill> _skillRepository;
        private readonly SkillProofDbContext _ctx;

        public SkillLogic(IRepository<SkillProof.Entities.Models.Skill> skillRepository, SkillProofDbContext _ctx)
        { 
            this._skillRepository = skillRepository;
            this._ctx = _ctx;
        }

        public async Task<ICollection<ViewSkill>> GetSkills()
        {
            var skills =  await _ctx.Skills.ToListAsync();

            return skills.Select(s => new ViewSkill
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();
        }

        public async Task AddSkillToTable(string skillName)
        {
            var newSkill = new SkillProof.Entities.Models.Skill();
            newSkill.Name = skillName;

            await _skillRepository.Create(newSkill);
        }
    }
}
