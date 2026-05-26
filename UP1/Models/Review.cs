using System;

namespace UP1.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public virtual Book Book { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public string UserLogin { get; set; }     // для удобного отображения

        public string Text { get; set; }
        public int Rating { get; set; }           // от 1 до 5

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsFrozen { get; set; } = false;
    }
}