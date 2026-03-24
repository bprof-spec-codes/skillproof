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
            var createdJob = await _jobLogic.CreateJobAsync(dto, dto.CompanyId);
            return Ok(createdJob);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(string id, [FromBody] JobCreateDto dto)
        {
                var updatedJob = await _jobLogic.UpdateJobAsync(id, dto, dto.CompanyId);
                return Ok(updatedJob);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(string id, [FromQuery] string companyId)
        {
                await _jobLogic.DeleteJobAsync(id, companyId);
                return NoContent();
        }
}