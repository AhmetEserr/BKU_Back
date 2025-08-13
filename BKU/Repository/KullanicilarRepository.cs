
    using BKU.Data;
    using BKU.Models;
    using BKU.Repository.Interfaces;
    using Microsoft.EntityFrameworkCore;

    namespace BKU.Repository
    {
    public class KullanicilarRepository : IKullanicilarRepository
    {
        private readonly ApplicationDbContext _context;
        public KullanicilarRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Kullanicilar>> GetAllAsync(CancellationToken ct = default)
            => await _context.Kullanicilar.AsNoTracking().ToListAsync(ct);

        public async Task<Kullanicilar?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _context.Kullanicilar.AsNoTracking()
                                          .FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task<Kullanicilar> AddAsync(Kullanicilar kullanici, CancellationToken ct = default)
        {
            // Basit e-posta çakışma kontrolü (opsiyonel ama faydalı)
            if (!string.IsNullOrWhiteSpace(kullanici.Email))
            {
                var exists = await _context.Kullanicilar.AsNoTracking()
                    .AnyAsync(x => x.Email == kullanici.Email, ct);
                if (exists)
                    throw new InvalidOperationException("Bu e-posta zaten kayıtlı.");
            }

            _context.Kullanicilar.Add(kullanici);
            await _context.SaveChangesAsync(ct);
            return kullanici;
        }

        public async Task<bool> UpdateAsync(Kullanicilar updated, CancellationToken ct = default)
        {
            var ent = await _context.Kullanicilar.FirstOrDefaultAsync(x => x.Id == updated.Id, ct);
            if (ent == null) return false;

            // Alan güncellemeleri
            ent.Username = updated.Username;
            ent.Email = updated.Email;
            ent.Parola = updated.Parola; // Not: JWT aşamasında hash'leyeceğiz
            ent.Role = updated.Role;

            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var ent = await _context.Kullanicilar.FindAsync(new object?[] { id }, ct);
            if (ent == null) return false;

            _context.Kullanicilar.Remove(ent);
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}
