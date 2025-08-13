using BKU.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BKU.Models;

namespace BKU.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAnswersController : ControllerBase
    {
        private readonly IAnswerRepository _answerRepo;
        private readonly IUserAnswerRepository _uaRepo;

        public UserAnswersController(IAnswerRepository answerRepo, IUserAnswerRepository uaRepo)
        {
            _answerRepo = answerRepo;
            _uaRepo = uaRepo;
        }

        public record SubmitRequest(int QuestionId, int AnswerId);

        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitRequest req, CancellationToken ct)
        {
            // 1) Answer doğrulama
            var answer = await _answerRepo.GetByIdAsync(req.AnswerId, ct);
            if (answer is null || answer.QuestionId != req.QuestionId)
                return BadRequest(new { message = "Geçersiz şık veya soru-şık uyuşmuyor." });

            // 2) (Varsa) kimlikten userId al
            int? userId = null;
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var parsed)) userId = parsed;

            // 3) Kaydet / güncelle
            var saved = await _uaRepo.UpsertAsync(new UserAnswer
            {
                UserId = userId,
                QuestionId = req.QuestionId,
                AnswerId = req.AnswerId
            }, ct);

            return Ok(new
            {
                correct = answer.IsCorrect,
                saved.Id,
                saved.UserId,
                saved.QuestionId,
                saved.AnswerId
            });
        }
    }
}
