using Microsoft.AspNetCore.Mvc;
using SkillProof.Entities.Dtos.Questions;
using SkillProof.Logic.Questions;

namespace SkillProof.Api.Controllers
{
    [ApiController]
    [Route("api/question-bank")]
    public class QuestionBankController : ControllerBase
    {
        private readonly IQuestionBankService _questionBankService;

        public QuestionBankController(IQuestionBankService questionBankService)
        {
            _questionBankService = questionBankService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(QuestionResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<QuestionResponseDto>> Create([FromBody] CreateQuestionRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                var created = await _questionBankService.CreateAsync(request, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<QuestionResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<QuestionResponseDto>>> GetAll([FromQuery] QuestionListFilterDto filter, CancellationToken cancellationToken)
        {
            var questions = await _questionBankService.GetAllAsync(filter, cancellationToken);
            return Ok(questions);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(QuestionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuestionResponseDto>> GetById(string id, CancellationToken cancellationToken)
        {
            var question = await _questionBankService.GetByIdAsync(id, cancellationToken);
            return question == null ? NotFound() : Ok(question);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(QuestionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuestionResponseDto>> Update(string id, [FromBody] UpdateQuestionRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                var updated = await _questionBankService.UpdateAsync(id, request, cancellationToken);
                return updated == null ? NotFound() : Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            var deleted = await _questionBankService.DeleteAsync(id, cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}
