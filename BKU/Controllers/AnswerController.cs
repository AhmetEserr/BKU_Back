

    using BKU.Models;
    using BKU.Repository.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    namespace BKU.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class AnswerController : ControllerBase
        {
            private readonly IAnswerRepository _repository;
            public AnswerController(IAnswerRepository repository) => _repository = repository;

            // GET: /api/Answer
            [HttpGet]
            public async Task<IActionResult> GetAll(CancellationToken ct)
            {
                var answers = await _repository.GetAllAsync(ct);
                return Ok(answers);
            }

            // GET: /api/Answer/5
            [HttpGet("{id:int}")]
            public async Task<IActionResult> GetById(int id, CancellationToken ct)
            {
                var answer = await _repository.GetByIdAsync(id, ct);
                return answer is null ? NotFound() : Ok(answer);
            }

            // POST: /api/Answer
            [HttpPost]
            public async Task<IActionResult> Create([FromBody] Answer answer, CancellationToken ct)
            {
                var created = await _repository.AddAsync(answer, ct);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }

            // PUT: /api/Answer/5
            [HttpPut("{id:int}")]
            public async Task<IActionResult> Update(int id, [FromBody] Answer answer, CancellationToken ct)
            {
                if (id != answer.Id)
                    return BadRequest("URL'deki id ile gövdedeki id uyuşmuyor.");

                var ok = await _repository.UpdateAsync(answer, ct);
                return ok ? NoContent() : NotFound();
            }

            // DELETE: /api/Answer/5
            [HttpDelete("{id:int}")]
            public async Task<IActionResult> Delete(int id, CancellationToken ct)
            {
                var ok = await _repository.DeleteAsync(id, ct);
                return ok ? NoContent() : NotFound();
            }
        }
    }
