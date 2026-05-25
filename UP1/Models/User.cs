using System;

namespace UP1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }

        public int RoleId { get; set; }
        public virtual Role Role { get; set; }

        public bool IsFrozen { get; set; } = false;
        public string FreezeReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Новые поля для заявки на автора
        public bool HasAuthorRequest { get; set; } = false;
        public string AuthorRequestStatus { get; set; } = "None"; // None, Pending, Approved, Rejected
        public string AuthorRequestReason { get; set; }
        public DateTime? AuthorRequestDate { get; set; }
    }
}