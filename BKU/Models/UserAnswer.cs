using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BKU.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }

        // Kullanıcı takibi yoksa nullable bırak (anonim cevap)
        public int? UserId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        public int AnswerId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigations
        [JsonIgnore] public Kullanicilar? User { get; set; }
        [JsonIgnore] public Question? Question { get; set; }
        [JsonIgnore] public Answer? Answer { get; set; }
    }
}
