using SkillProof.Entities.Dtos.Tests;
using SkillProof.Entities.Dtos.Users;

namespace SkillProof.Logic.User;

public interface IUserLogic
{
    Task RegisterUserAsync(RegisterUser dto);
    Task RegisterEmployerAsync(RegisterEmployer dto);
    Task<IEnumerable<ViewUser>> GetAllUsersAsync();
    Task<ViewUser> GetUserByIdAsync(string id);
    Task UpdateUserAsync(string id, UpdateUser dto);
    Task DeleteUserAsync(string id);
    Task<LoginResultDto> LoginAsync(LoginUser dto);
    Task GrantAdminRoleAsync(string userId);
    Task RevokeRoleAsync(string userId);
    Task<IEnumerable<UserTestsDto>> GetUserTestsAsync(string userId);
    Task UpdateSkillsToUser(string id, UpdateSkillToUser dto);
}