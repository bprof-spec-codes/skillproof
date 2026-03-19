using SkillProof.Entities.Dtos.Questions;

namespace SkillProof.Logic.Questions
{
    public interface IQuestionBankService
    {
        Task<QuestionResponseDto> CreateAsync(CreateQuestionRequestDto request, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<QuestionResponseDto>> GetAllAsync(QuestionListFilterDto filter, CancellationToken cancellationToken = default);
        Task<QuestionResponseDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<QuestionResponseDto?> UpdateAsync(string id, UpdateQuestionRequestDto request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
