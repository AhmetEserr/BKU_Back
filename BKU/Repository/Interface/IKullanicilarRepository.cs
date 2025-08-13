using BKU.Models;

namespace BKU.Repository.Interfaces
{
    public interface IKullanicilarRepository
    {
        Task<IEnumerable<Kullanicilar>> GetAllAsync(CancellationToken ct = default);
        Task<Kullanicilar?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Kullanicilar> AddAsync(Kullanicilar kullanici, CancellationToken ct = default);
        Task<bool> UpdateAsync(Kullanicilar updatedKullanici, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
