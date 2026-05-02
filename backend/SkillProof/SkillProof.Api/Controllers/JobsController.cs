using Microsoft.AspNetCore.Mvc;
using SkillProof.Entities.Dtos.Assesment;
using SkillProof.Entities.Dtos.Job;
using SkillProof.Entities.Dtos.Jobs;
using SkillProof.Entities.Dtos.Questions;
using SkillProof.Entities.Models;
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
        var companyId = User.Claims.FirstOrDefault(c => 
            c.Type == "CompanyId" || 
            c.Type.EndsWith("CompanyId", StringComparison.OrdinalIgnoreCase))?.Value;

        if (string.IsNullOrEmpty(companyId))
        {
            return BadRequest(new { message = "There is no company id like this" });
        }
        
        var result = await _jobLogic.CreateJobAsync(dto, companyId);
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllJobs()
    {
        var jobs = await _jobLogic.GetAllJobsAsync();
        return Ok(jobs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobById(string id)
    {
        var job = await _jobLogic.GetJobByIdAsync(id);
        return Ok(job);
    }
    
    [HttpGet("company/{companyId}")]
    public async Task<IActionResult> GetJobsByCompany(string companyId)
    {
        if (string.IsNullOrWhiteSpace(companyId))
        {
            return BadRequest(new { message = "Company ID is required." });
        }

        var jobs = await _jobLogic.GetJobsByCompanyIdAsync(companyId);
        return Ok(jobs);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateJob(string id, [FromBody] JobViewDto dto)
    {
        var companyId = User.Claims.FirstOrDefault(c => 
            c.Type == "CompanyId" || 
            c.Type.EndsWith("CompanyId", StringComparison.OrdinalIgnoreCase))?.Value;

        if (string.IsNullOrEmpty(companyId))
        {
            return BadRequest(new { message = "Company ID is missing from the authentication token." });
        }

        var updatedJob = await _jobLogic.UpdateJobAsync(id, dto, companyId);
        return Ok(updatedJob);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJob(string id, [FromQuery] string companyId)
    {
        await _jobLogic.DeleteJobAsync(id, companyId);
        return NoContent();
    }

    [HttpGet("GetTestToJob{id}")]

    public async Task<ActionResult<ICollection<AssessmentViewDto>>> GetRndQuestions(string id)
    {
        var test = await _jobLogic.GetTestToJob(id);
        return test == null ? NotFound() : Ok(test);
    }

    [HttpGet("{id}/test")]
    public async Task<IActionResult> GetCandidateTest(string id)
    {
        var candidateTest = await _jobLogic.GetCandidateTestForJob(id);
        if (candidateTest == null)
        {
            return NoContent();
        }
        return Ok(candidateTest);
    }

    [HttpPost("{id}/apply")]
    public async Task<IActionResult> ApplyForJob(string id)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "You must be logged in to apply." });
        }

        try
        {
            var applicationId = await _jobLogic.ApplyForJobAsync(id, userId);
            return Ok(new { ApplicationId = applicationId, message = "Application submitted successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while submitting the application.", details = ex.Message });
        }
    }
}