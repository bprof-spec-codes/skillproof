using Microsoft.AspNetCore.Mvc;
using SkillProof.Entities.Dtos.Tests;
using SkillProof.Logic.Tests;
using System.Security.Claims;

namespace SkillProof.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestsController : ControllerBase
{
    private readonly ITestLogic _testLogic;

    public TestsController(ITestLogic testLogic)
    {
        _testLogic = testLogic;
    }

    [HttpPost("submit")]
    public async Task<ActionResult<TestResultDto>> SubmitTest([FromBody] TestSubmitDto dto)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "You must be logged in to submit a test." });
        }

        var result = await _testLogic.SubmitTestAsync(dto, userId);
        return Ok(result);
    }
}
