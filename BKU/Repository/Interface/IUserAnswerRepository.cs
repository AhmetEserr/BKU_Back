/*namespace BKU.Repository.Interface
{
    public class IUserAnswerRepository
    {
    }
}
*/
using BKU.Models;

namespace BKU.Repository.Interfaces
{
    public interface IUserAnswerRepository
    {
        Task<UserAnswer> UpsertAsync(UserAnswer ua, CancellationToken ct = default);
        Task<UserAnswer?> GetAsync(int? userId, int questionId, CancellationToken ct = default);
        Task<int> CountCorrectByUserAsync(int userId, CancellationToken ct = default);
    }
}
