
﻿using System.ComponentModel.DataAnnotations;


namespace BKU.Models
{
    public class Kullanicilar
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Parola { get; set; } = string.Empty; // (ileride hash)

        [MaxLength(50)]
        public string? Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
