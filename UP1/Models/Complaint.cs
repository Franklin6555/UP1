using System;

namespace UP1.Models
{
    public class Complaint
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int? BookId { get; set; }
        public virtual Book Book { get; set; }

        public string Reason { get; set; }

        public bool IsResolved { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}