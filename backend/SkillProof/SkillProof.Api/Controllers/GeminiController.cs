using Microsoft.AspNetCore.Mvc;
using SkillProof.Entities.Dtos.Gemini;
using SkillProof.Entities.Models.Gemini;
using SkillProof.Logic.Gemini;

namespace SkillProof.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GradingController : ControllerBase
    {
        private readonly IGeminiService _gradingService;

        public GradingController(IGeminiService gradingService)
        {
            _gradingService = gradingService;
        }

        [HttpPost("grade")]
        public async Task<IActionResult> Grade([FromBody] GradingRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.StudentAnswer))
                return BadRequest("Student answer cannot be empty.");

            var result = await _gradingService.EvaluateAnswerAsync(request);
            return Ok(result);
        }

        [HttpPost("grade-multiple")]
        public async Task<IActionResult> GradeMultiple([FromBody] List<GradingRequest> requests)
        {
            if (requests == null || !requests.Any())
                return BadRequest("Request list cannot be empty.");
            var results = new List<double>();
            foreach (var request in requests)
            {
                if (string.IsNullOrWhiteSpace(request.StudentAnswer))
                    return BadRequest("Student answer cannot be empty.");
                var score = await _gradingService.EvaluateAnswerAsync(request);
                results.Add(score);
            }
            return Ok(results);
        }
    }
}
