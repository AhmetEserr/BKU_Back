
using BKU.Models;
using Microsoft.EntityFrameworkCore;

namespace BKU.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Kullanicilar> Kullanicilar => Set<Kullanicilar>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<Answer> Answers => Set<Answer>();

        public DbSet<UserAnswer> UserAnswers => Set<UserAnswer>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // Kullanicilar
            b.Entity<Kullanicilar>(e =>
            {
                e.Property(x => x.Username).IsRequired().HasMaxLength(100);
                e.Property(x => x.Email).IsRequired().HasMaxLength(200);
                e.Property(x => x.Parola).IsRequired().HasMaxLength(200);
                e.Property(x => x.Role).HasMaxLength(50).HasDefaultValue("User");
                e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                e.HasIndex(x => x.Email).IsUnique(); // benzersiz e-posta
            });

            // Question
            b.Entity<Question>(e =>
            {
                e.Property(x => x.Text).IsRequired().HasMaxLength(500);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            });

            // Answer
            b.Entity<Answer>(e =>
            {
                e.Property(x => x.Text).IsRequired().HasMaxLength(500);
                e.Property(x => x.IsCorrect).HasDefaultValue(false);

                e.HasOne(a => a.Question)
                 .WithMany(q => q.Answers)
                 .HasForeignKey(a => a.QuestionId)
                 .OnDelete(DeleteBehavior.Cascade); // Question silinirse Answer’lar silinsin
                e.HasIndex(a => a.QuestionId);
            });
            b.Entity<UserAnswer>(e =>
            {
                e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

                e.HasOne(x => x.User)
                 .WithMany() // istersen User'a ICollection<UserAnswer> ekleyebilirsin
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(x => x.Question)
                 .WithMany() // Question tarafında koleksiyon istemiyorsan böyle kalabilir
                 .HasForeignKey(x => x.QuestionId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Answer)
                 .WithMany()
                 .HasForeignKey(x => x.AnswerId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Aynı kullanıcı aynı soruya bir kez cevap verebilsin (anonimler hariç)
                e.HasIndex(x => new { x.UserId, x.QuestionId })
                 .IsUnique()
                 .HasFilter("[UserId] IS NOT NULL");

                // Yardımcı indeksler
                e.HasIndex(x => x.QuestionId);
                e.HasIndex(x => x.AnswerId);
            });

            // (Opsiyonel) Seed örneği – Migration’a yazar
            // b.Entity<Kullanicilar>().HasData(new Kullanicilar {
            //     Id = 1, Username = "admin", Email = "admin@example.com", Parola = "123456", Role = "Admin"
            // });
        }
    }
}
