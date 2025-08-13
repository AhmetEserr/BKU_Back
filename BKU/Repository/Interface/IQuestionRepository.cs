using BKU.Models;

namespace BKU.Repository.Interfaces
{
    public interface IQuestionRepository
    {
        Task<List<Question>> GetAllAsync(CancellationToken ct = default);
        Task<Question?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Question> AddAsync(Question question, CancellationToken ct = default);
        Task<bool> UpdateAsync(Question question, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
