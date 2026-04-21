using Microsoft.AspNetCore.Mvc;
using SkillProof.Logic.Helper;

namespace SkillProof.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarkdownController : ControllerBase
    {
        private readonly IMarkdownService _markdownService;

        public MarkdownController(IMarkdownService markdownService)
        {
            _markdownService = markdownService;
        }

        [HttpPost("PreView")]
        public async Task<IActionResult> ConvertMarkdownToHtml([FromBody] string text)
        {
            string html = _markdownService.ToHtml(text);
            return Ok(html);
        }
    }
}
