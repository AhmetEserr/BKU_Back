using BKU.Models;

namespace BKU.Repository.Interfaces
{
    public interface IAnswerRepository
    {
        Task<List<Answer>> GetAllAsync(CancellationToken ct = default);
        Task<Answer?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Answer> AddAsync(Answer answer, CancellationToken ct = default);
        Task<bool> UpdateAsync(Answer answer, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
