using Microsoft.EntityFrameworkCore;
using SkillProof.Data.Repositorys;
using SkillProof.Entities.Dtos.Education;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillProof.Entities.Models;

namespace SkillProof.Logic.Education
{
    public class EducationLogic : IEducationLogic
    {
        private readonly IRepository<SkillProof.Entities.Models.Education> _educationRepository;
        private readonly IRepository<Users> _userRepository;

        public EducationLogic(IRepository<SkillProof.Entities.Models.Education> educationRepository, IRepository<Users> userRepository)
        {
            _educationRepository = educationRepository;
            _userRepository = userRepository;
        }
        public async Task DeleteEducationAsync(string id, string userId)
        {
            var education = await _educationRepository.GetOne(id);
            if (education == null)
            {
                throw new Exception("Education not found");
            }
            var user = await _userRepository.GetOne(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            await _educationRepository.DeleteById(id);
        }

        public async Task<IEnumerable<EducationViewDto>> GetEducationsByUserIdAsync(string userId)
        {
            var user = await _userRepository.GetOne(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var educations = await _educationRepository.GetAll()
                .Where(e => e.UserId == userId)
                .Select(e => new EducationViewDto
                {
                    Id = e.Id,
                    School = e.School,
                    Degree = e.Degree,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Description = e.Description,
                }).ToListAsync();
            return educations;
        }

        public async Task<EducationCreateDto> CreateEducationAsync(EducationCreateDto entity, string userId)
        {
            var user = await _userRepository.GetOne(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            if (entity.StartDate < new DateTime(1753, 1, 1))
            {
                throw new Exception("StartDate must be a valid date.");
            }

            var education = new SkillProof.Entities.Models.Education
            {
                UserId = userId,
                School = entity.School,
                Degree = entity.Degree,
                FieldOfStudy = entity.FieldOfStudy,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Description = entity.Description
            };

            await _educationRepository.Create(education);
            return entity;
        }
    }
}