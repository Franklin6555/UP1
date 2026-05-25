using System;

namespace UP1.Models
{
    public class UserBookList
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int BookId { get; set; }
        public virtual Book Book { get; set; }

        public int StatusId { get; set; }
        public virtual ReadingStatus Status { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}