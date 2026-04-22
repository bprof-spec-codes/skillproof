using SkillProof.Entities.Dtos.Tests;

namespace SkillProof.Logic.Tests;

public interface ITestLogic
{
    Task<TestResultDto> SubmitTestAsync(TestSubmitDto dto, string userId);
}
