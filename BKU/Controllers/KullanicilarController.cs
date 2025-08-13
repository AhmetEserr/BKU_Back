
using BKU.Models;
using BKU.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BKU.Controllers
{
    // Response'ta parola dönmemek için basit DTO
    public record UserDto(int Id, string? Username, string? Email, string? Role);

    [ApiController]
    [Route("api/[controller]")]
    public class KullanicilarController : ControllerBase
    {
        private readonly IKullanicilarRepository _repository;
        public KullanicilarController(IKullanicilarRepository repository) => _repository = repository;

        // GET: /api/Kullanicilar
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(CancellationToken ct)
        {
            var users = await _repository.GetAllAsync(ct);
            var dtos = users.Select(u => new UserDto(u.Id, u.Username, u.Email, u.Role)).ToList();
            return Ok(dtos);
        }

        // GET: /api/Kullanicilar/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetById(int id, CancellationToken ct)
        {
            var u = await _repository.GetByIdAsync(id, ct);
            if (u is null) return NotFound();

            return Ok(new UserDto(u.Id, u.Username, u.Email, u.Role));
        }

        // POST: /api/Kullanicilar
        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] Kullanicilar model, CancellationToken ct)
        {
            // Not: Parola şu an düz metin; JWT'ye geçtiğinde hash'leyeceğiz.
            var created = await _repository.AddAsync(model, ct);
            var dto = new UserDto(created.Id, created.Username, created.Email, created.Role);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, dto);
        }

        // PUT: /api/Kullanicilar/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Kullanicilar model, CancellationToken ct)
        {
            if (id != model.Id) return BadRequest("URL'deki id ile gövdedeki id uyuşmuyor.");

            var ok = await _repository.UpdateAsync(model, ct);
            return ok ? NoContent() : NotFound();
        }

        // DELETE: /api/Kullanicilar/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var ok = await _repository.DeleteAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }
    }
}
