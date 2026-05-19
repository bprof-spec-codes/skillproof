using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillProof.Entities.Dtos.Users;
using System.Security.Claims;
using SkillProof.Logic.User;

namespace SkillProof.Api.Controllers
{
   [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserLogic _userLogic;

        public UserController(IUserLogic userLogic)
        {
            _userLogic = userLogic;
        }   

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUser dto)
        {
                await _userLogic.RegisterUserAsync(dto);
                return Ok(new { message = "Registration successful." });
        }
        
        [HttpPost("RegisterEmployer")]
        public async Task<IActionResult> RegisterEmployer([FromBody] RegisterEmployer dto)
        {
                await _userLogic.RegisterEmployerAsync(dto);
                return Ok();
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
                var users = await _userLogic.GetAllUsersAsync();
                return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
                var user = await _userLogic.GetUserByIdAsync(id);
                return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUser dto)
        {
                await _userLogic.UpdateUserAsync(id, dto);
                return Ok(new { message = "User updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
                await _userLogic.DeleteUserAsync(id);
                return NoContent();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUser dto)
        {
                var result = await _userLogic.LoginAsync(dto);
                return Ok(result);
        }

        [HttpGet("GrantAdmin/{userId}")]
        public async Task<IActionResult> GrantAdminRole(string userId)
        {
                await _userLogic.GrantAdminRoleAsync(userId);
                return Ok(new { message = "Admin role granted." });
            
        }

        [HttpGet("RevokeRole/{userId}")]
        public async Task<IActionResult> RevokeRole(string userId)
        {
                await _userLogic.RevokeRoleAsync(userId);
                return Ok(new { message = "Roles revoked successfully." });
        }

        [HttpGet("UserTests/{userId}")]
        public async Task<IActionResult> GetUserTests(string userId)
        {
                var tests = await _userLogic.GetUserTestsAsync(userId);
                return Ok(tests);
        }

        [HttpPost("{id}/skills")]
        public async Task<IActionResult> UpdateSkillsToUser(string id, [FromBody] string[] skillId)
        {
            try
            {
                await _userLogic.UpdateSkillsToUser(id, skillId);
                return Ok(new { message = "Skill successfully added to user." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("toggle-saved-job/{jobId}")]
        public async Task<IActionResult> ToggleSavedJob(string jobId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var updatedProfile = await _userLogic.ToggleSavedJobAsync(currentUserId, jobId);
            return Ok(updatedProfile);
        }

        [HttpPost("apply/{jobId}")]
        public async Task<IActionResult> ApplyToJob(string jobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                await _userLogic.ApplyToJobAsync(userId, jobId);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("remove-skill/{skillId}/{userId}")]
        public async Task<IActionResult> RemoveSkill(string skillId, string userId)
        {
            try
            {
                await _userLogic.DeleteSkillFromUser(userId, skillId);
                return Ok(new { message = "Skill removed successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
