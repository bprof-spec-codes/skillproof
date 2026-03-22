using Microsoft.AspNetCore.Mvc;
using SkillProof.Entities.Dtos.Job;
using SkillProof.Logic.Jobs;

namespace SkillProof.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class JobsController : ControllerBase
{
    private readonly IJobLogic _jobLogic;

    public JobsController(IJobLogic jobLogic)
    {
        _jobLogic = jobLogic;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateJob([FromBody] JobCreateDto dto)
    {
        try
        {
            var createdJob = await _jobLogic.CreateJobAsync(dto, dto.CompanyId);

            return Ok(createdJob);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}