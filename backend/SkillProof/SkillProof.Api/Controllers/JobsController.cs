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
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
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
            if (job == null)
            {
                return NotFound(new { error = "The job is not found." });
            }
            
            return Ok(job);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(string id, [FromBody] JobCreateDto dto)
        {
            try
            {
                var updatedJob = await _jobLogic.UpdateJobAsync(id, dto, dto.CompanyId);
                return Ok(updatedJob);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, new { error = "You do not have permission to modify this job." });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(string id, [FromQuery] string companyId)
        {
            try
            {
                await _jobLogic.DeleteJobAsync(id, companyId);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, new { error = "You do not have permission to delete this job." });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
}