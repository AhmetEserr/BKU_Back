

using BKU.Data;
using BKU.Models;
using BKU.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BKU.Repository
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly ApplicationDbContext _context;
        public AnswerRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<Answer>> GetAllAsync(CancellationToken ct = default) =>
            await _context.Answers.AsNoTracking().Include(a => a.Question).ToListAsync(ct);

        public async Task<Answer?> GetByIdAsync(int id, CancellationToken ct = default) =>
            await _context.Answers.AsNoTracking().Include(a => a.Question)
                                  .FirstOrDefaultAsync(a => a.Id == id, ct);

        public async Task<Answer> AddAsync(Answer answer, CancellationToken ct = default)
        {
            await _context.Answers.AddAsync(answer, ct);
            await _context.SaveChangesAsync(ct);
            return answer;
        }

        public async Task<bool> UpdateAsync(Answer answer, CancellationToken ct = default)
        {
            var ent = await _context.Answers.FirstOrDefaultAsync(x => x.Id == answer.Id, ct);
            if (ent == null) return false;
            ent.Text = answer.Text;
            ent.IsCorrect = answer.IsCorrect;
            ent.QuestionId = answer.QuestionId;
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var ent = await _context.Answers.FindAsync(new object?[] { id }, ct);
            if (ent == null) return false;
            _context.Answers.Remove(ent);
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}

