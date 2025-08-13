

using BKU.Hubs;
using BKU.Models;
using BKU.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BKU.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionRepository _repository;
        private readonly IHubContext<QuizHub> _hub;

        public QuestionController(IQuestionRepository repository, IHubContext<QuizHub> hub)
        {
            _repository = repository;
            _hub = hub;
        }

        // GET: /api/Question
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var questions = await _repository.GetAllAsync(ct);
            return Ok(questions);
        }

        // GET: /api/Question/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var question = await _repository.GetByIdAsync(id, ct);
            return question is null ? NotFound() : Ok(question);
        }

        // POST: /api/Question
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Question question, CancellationToken ct)
        {
            var created = await _repository.AddAsync(question, ct);

            // Tüm istemcilere yayın
            await _hub.Clients.All.SendAsync("QuestionAdded",
                new { created.Id, created.Text }, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: /api/Question/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Question question, CancellationToken ct)
        {
            if (id != question.Id)
                return BadRequest("URL'deki id ile gövdedeki id uyuşmuyor.");

            var ok = await _repository.UpdateAsync(question, ct);
            if (!ok) return NotFound();

            await _hub.Clients.All.SendAsync("QuestionUpdated",
                new { question.Id, question.Text }, ct);

            return NoContent();
        }

        // DELETE: /api/Question/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var ok = await _repository.DeleteAsync(id, ct);
            if (!ok) return NotFound();

            await _hub.Clients.All.SendAsync("QuestionDeleted", new { id }, ct);

            return NoContent();
        }
    }
}
