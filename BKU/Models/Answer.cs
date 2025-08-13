
﻿using System.ComponentModel.DataAnnotations;


namespace BKU.Models
{
    public class Answer
    {
        public int Id { get; set; }

        [Required, MaxLength(500)]
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
