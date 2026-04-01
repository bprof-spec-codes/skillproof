using Microsoft.AspNetCore.Mvc;
using SkillProof.Entities.Dtos.Job;
using SkillProof.Entities.Dtos.Jobs;
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
        // 1. Megnézzük, mi jött át az Angulartól a Headerben
        var authHeader = Request.Headers["Authorization"].ToString();
        Console.WriteLine("---------------------------------------------------");
        Console.WriteLine($"1. BEÉRKEZŐ HEADER: {authHeader}");
        Console.WriteLine("---------------------------------------------------");

        // 2. Megnézzük, a C# mikre bontotta szét
        Console.WriteLine("2. C# ÁLTAL LÁTOTT CLAIMEK:");
        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"   Kulcs: '{claim.Type}' | Érték: '{claim.Value}'");
        }
        Console.WriteLine("---------------------------------------------------");

        // 3. Megpróbáljuk kivenni a CompanyId-t (minden lehetséges módon)
        var companyId = User.Claims.FirstOrDefault(c => 
            c.Type == "CompanyId" || 
            c.Type.EndsWith("CompanyId", StringComparison.OrdinalIgnoreCase))?.Value;

        if (string.IsNullOrEmpty(companyId))
        {
            // Ha ide lép, a terminálban lévő logból azonnal látni fogjuk, miért!
            return BadRequest(new { message = "Backend nem látja a CompanyId-t. Nézd meg a Rider terminálját!" });
        }

        try 
        {
            // Ha megvan, megyünk a Logic-ba
            var result = await _jobLogic.CreateJobAsync(dto, companyId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
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
        return Ok(job);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateJob(string id, [FromBody] JobViewDto dto)
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