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

        public string ReviewText { get; set; }
        public int Rating { get; set; }

        public bool IsFrozen { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}