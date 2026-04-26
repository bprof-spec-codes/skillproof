using SkillProof.Entities.Dtos.Tests;

namespace SkillProof.Logic.Tests;

public interface ITestLogic
{
    Task<TestResultDto> SubmitTestAsync(TestSubmitDto dto, string userId);
    Task<List<UserTestReviewDto>> GetUserTestQuestionsAsync(string jobId, string userId);
    Task<FeedbackResponseDto> ManualFeedbackAsync(string? feedback, double score, string testAnswerId);
    Task<List<string?>> GetTestUsersAsync(string jobId);
}
