using Microsoft.EntityFrameworkCore;
using SkillProof.Data.Repositorys;
using SkillProof.Entities.Dtos.Experience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SkillProof.Entities.Models;

namespace SkillProof.Logic.Experience
{
    public class ExperienceLogic : IExperienceLogic
    {
        private readonly IRepository<UserExperiences> _experienceRepository;
        private readonly IRepository<Users> _userRepository;

        public ExperienceLogic(IRepository<UserExperiences> experienceRepository, IRepository<Users> userRepository)
        {
            _experienceRepository = experienceRepository;
            _userRepository = userRepository;
        }

        public async Task DeleteExperienceAsync(string id, string userId)
        {
            var experience = await _experienceRepository.GetOne(id);
            if (experience == null)
            {
                throw new Exception("Experience not found");
            }

            var user = await _userRepository.GetOne(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            await _experienceRepository.DeleteById(id);
        }

        public async Task<IEnumerable<ExperienceViewDto>> GetExperiencesByUserIdAsync(string userId)
        {
            var user = await _userRepository.GetOne(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var experiences = await _experienceRepository.GetAll()
                .Where(e => e.UserId == userId)
                .Select(e => new ExperienceViewDto
                {
                    Id = e.Id,
                    CompanyName = e.CompanyName,
                    JobTitle = e.JobTitle,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate
                })
                .ToListAsync();

            return experiences;
        }

        public async Task<ExperienceCreateDto> CreateExperienceAsync(ExperienceCreateDto entity, string userId)
        {
            var user = await _userRepository.GetOne(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var experience = new UserExperiences
            {
                UserId = userId,
                CompanyName = entity.CompanyName,
                JobTitle = entity.JobTitle,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate
            };

            await _experienceRepository.Create(experience);

            return entity;
        }
    }
}