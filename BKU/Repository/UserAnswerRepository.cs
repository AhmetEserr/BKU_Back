using BKU.Data;
using BKU.Models;
using BKU.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BKU.Repository
{
    public class UserAnswerRepository : IUserAnswerRepository
    {
        private readonly ApplicationDbContext _ctx;
        public UserAnswerRepository(ApplicationDbContext ctx) => _ctx = ctx;

        public async Task<UserAnswer?> GetAsync(int? userId, int questionId, CancellationToken ct = default)
        {
            return await _ctx.UserAnswers.AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.QuestionId == questionId, ct);
        }

        // Varsa günceller, yoksa ekler (UserId null ise her seferinde yeni kayıt)
        public async Task<UserAnswer> UpsertAsync(UserAnswer ua, CancellationToken ct = default)
        {
            if (ua.UserId.HasValue)
            {
                var existing = await _ctx.UserAnswers
                    .FirstOrDefaultAsync(x => x.UserId == ua.UserId && x.QuestionId == ua.QuestionId, ct);
                if (existing is not null)
                {
                    existing.AnswerId = ua.AnswerId;
                    existing.CreatedAt = DateTime.UtcNow;
                    await _ctx.SaveChangesAsync(ct);
                    return existing;
                }
            }

            _ctx.UserAnswers.Add(ua);
            await _ctx.SaveChangesAsync(ct);
            return ua;
        }

        public async Task<int> CountCorrectByUserAsync(int userId, CancellationToken ct = default)
        {
            return await _ctx.UserAnswers
                .Where(ua => ua.UserId == userId)
                .Join(_ctx.Answers, ua => ua.AnswerId, a => a.Id, (ua, a) => a.IsCorrect)
                .CountAsync(isCorrect => isCorrect, ct);
        }
    }
}
