
/*
using BKU.Data;
using BKU.Models;
using BKU.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BKU.Repository
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _context;
        public QuestionRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<Question>> GetAllAsync(CancellationToken ct = default) =>
            await _context.Questions.AsNoTracking()
                                    .Include(q => q.Answers)
                                    .ToListAsync(ct);

        public async Task<Question?> GetByIdAsync(int id, CancellationToken ct = default) =>
            await _context.Questions.AsNoTracking()
                                    .Include(q => q.Answers)
                                    .FirstOrDefaultAsync(q => q.Id == id, ct);

        public async Task<Question> AddAsync(Question question, CancellationToken ct = default)
        {
            await _context.Questions.AddAsync(question, ct);
            await _context.SaveChangesAsync(ct);
            return question;
        }

        public async Task<bool> UpdateAsync(Question question, CancellationToken ct = default)
        {
            var ent = await _context.Questions.FirstOrDefaultAsync(q => q.Id == question.Id, ct);
            if (ent == null) return false;

            ent.Text = question.Text;
            // Answers koleksiyonunu burada güncellemek istiyorsan ayrıca ele al:
            // ent.Answers = question.Answers;

            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var ent = await _context.Questions.FindAsync(new object?[] { id }, ct);
            if (ent == null) return false;

            _context.Questions.Remove(ent);
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}
*/

using BKU.Data;
using BKU.Models;
using BKU.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BKU.Repository
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _context;
        public QuestionRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<Question>> GetAllAsync(CancellationToken ct = default) =>
            await _context.Questions.AsNoTracking().Include(q => q.Answers).ToListAsync(ct);

        public async Task<Question?> GetByIdAsync(int id, CancellationToken ct = default) =>
            await _context.Questions.AsNoTracking().Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id, ct);

        public async Task<Question> AddAsync(Question question, CancellationToken ct = default)
        {
            await _context.Questions.AddAsync(question, ct);
            await _context.SaveChangesAsync(ct);
            return question;
        }

        public async Task<bool> UpdateAsync(Question question, CancellationToken ct = default)
        {
            var ent = await _context.Questions.FirstOrDefaultAsync(x => x.Id == question.Id, ct);
            if (ent == null) return false;
            ent.Text = question.Text;
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var ent = await _context.Questions.FindAsync(new object?[] { id }, ct);
            if (ent == null) return false;
            _context.Questions.Remove(ent);
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}
