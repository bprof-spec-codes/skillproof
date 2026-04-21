using Microsoft.EntityFrameworkCore;
using SkillProof.Data.Repositorys;
using SkillProof.Entities.Dtos.Assesment;
using SkillProof.Entities.Dtos.Questions;
using SkillProof.Logic.Assesments;


namespace SkillProof.Logic.Assessments
{
    public class AssessmentLogic : IAssessmentLogic
    {
        private readonly IRepository<Entities.Models.Assessments> _assessmentRepository;
        private readonly IRepository<Entities.Models.Questions> _questionRepository;

        public AssessmentLogic(
            IRepository<Entities.Models.Assessments> assessmentRepository,
            IRepository<Entities.Models.Questions> questionRepository)
        {
            _assessmentRepository = assessmentRepository;
            _questionRepository = questionRepository;
        }

        public async Task<AssessmentViewDto> CreateAssessmentAsync(CreateAssessmentDto model, string userId)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Assessment data cannot be null.");
            }

            var assessment = new Entities.Models.Assessments
            {
                Id = Guid.NewGuid().ToString(),
                Title = model.Title,
                Description = model.Description,
                DifficultyLevel = model.DifficultyLevel,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            if (model.QuestionIds != null && model.QuestionIds.Any())
            {
                var questions = await _questionRepository.GetAll()
                    .Where(q => model.QuestionIds.Contains(q.Id))
                    .ToListAsync();

                foreach (var question in questions)
                {
                    assessment.Questions.Add(question);
                }
            }

            await _assessmentRepository.Create(assessment);

            return await GetAssessmentByIdAsync(assessment.Id) 
                   ?? throw new InvalidOperationException("Failed to retrieve created assessment.");
        }

        public async Task<IEnumerable<AssessmentViewDto>> GetAllAssessmentsAsync()
        {
            var assessments = await _assessmentRepository.GetAll()
                .Include(a => a.Questions)
                .ToListAsync();

            return assessments.Select(MapToViewDto);
        }

        public async Task<AssessmentViewDto?> GetAssessmentByIdAsync(string id)
        {
            var assessment = await _assessmentRepository.GetAll()
                .Include(a => a.Questions)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assessment == null)
            {
                throw new KeyNotFoundException("Assessment not found.");
            }

            return MapToViewDto(assessment);
        }

        public async Task<AssessmentViewDto> UpdateAssessmentAsync(string id, UpdateAssessmentDto model, string userId)
        {
            var assessment = await _assessmentRepository.GetAll()
                .Include(a => a.Questions)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assessment == null)
            {
                throw new KeyNotFoundException("Assessment not found.");
            }

            assessment.Title = model.Title;
            assessment.Description = model.Description;
            assessment.DifficultyLevel = model.DifficultyLevel;
            assessment.IsActive = model.IsActive;

            assessment.Questions.Clear();

            if (model.QuestionIds != null && model.QuestionIds.Any())
            {
                var questions = await _questionRepository.GetAll()
                    .Where(q => model.QuestionIds.Contains(q.Id))
                    .ToListAsync();

                foreach (var question in questions)
                {
                    assessment.Questions.Add(question);
                }
            }

            await _assessmentRepository.Update(assessment);

            return MapToViewDto(assessment);
        }

        public async Task DeleteAssessmentAsync(string id)
        {
            var assessment = await _assessmentRepository.GetOne(id);
            if (assessment == null)
            {
                throw new KeyNotFoundException("Assessment not found.");
            }

            await _assessmentRepository.DeleteById(id);
        }

        private static AssessmentViewDto MapToViewDto(Entities.Models.Assessments assessment)
        {
            return new AssessmentViewDto
            {
                Id = assessment.Id,
                Title = assessment.Title,
                Description = assessment.Description,
                DifficultyLevel = assessment.DifficultyLevel,
                CreatedBy = assessment.CreatedBy,
                CreatedAt = assessment.CreatedAt,
                IsActive = assessment.IsActive,
                QuestionIds = assessment.Questions.Select(q => q.Id).ToList(),
                Questions = assessment.Questions.Select(q => new QuestionResponseDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Type = q.Type,
                    Difficulty = q.Difficulty,
                    Language = q.Language
                }).ToList()
            };
        }
    }
}