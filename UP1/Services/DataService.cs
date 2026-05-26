using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using UP1.Data;
using UP1.Models;

namespace UP1.Services
{
    public class DataService
    {
        private readonly AppDbContext db = new AppDbContext();

        public User GetUser(string login, string password)
        {
            return db.Users.Include(u => u.Role)
                           .FirstOrDefault(u => u.Login == login && u.PasswordHash == password);
        }

        public List<Book> GetAllBooks() => db.Books.Include(b => b.Author).ToList();

        public void AddBook(Book book)
        {
            db.Books.Add(book);
            db.SaveChanges();
        }

        public void UpdateBook(Book book)
        {
            db.Entry(book).State = EntityState.Modified;
            db.SaveChanges();
        }
        public List<Book> GetBooksByGenre(string genre)
        {
            if (string.IsNullOrEmpty(genre))
                return GetAllBooks();

            return db.Books
                     .Include(b => b.Author)
                     .Where(b => b.Genre == genre)
                     .ToList();
        }
        public List<Book> GetBooksOnShelf(int userId, string statusName)
        {
            return db.UserBookLists
                     .Where(ubl => ubl.UserId == userId && ubl.Status.Name == statusName)
                     .Select(ubl => ubl.Book)
                     .Include(b => b.Author)
                     .ToList();
        }
        public List<User> GetAllUsers()
        {
            return db.Users.Include(u => u.Role).ToList();
        }

        public RegisterResult RegisterUser(string login, string password, string displayName, string email)
        {
            try
            {
                if (db.Users.Any(u => u.Login == login))
                    return new RegisterResult { Success = false, Message = "Пользователь с таким логином уже существует." };

                if (db.Users.Any(u => u.Email == email))
                    return new RegisterResult { Success = false, Message = "Пользователь с таким email уже существует." };

                var userRole = db.Roles.FirstOrDefault(r => r.Name == "User");
                if (userRole == null)
                    return new RegisterResult { Success = false, Message = "Ошибка системы: роль User не найдена." };

                var newUser = new User
                {
                    Login = login,
                    PasswordHash = password,
                    DisplayName = displayName,
                    Email = email,
                    RoleId = userRole.Id
                };

                db.Users.Add(newUser);
                db.SaveChanges();

                return new RegisterResult { Success = true, Message = "Регистрация успешна" };
            }
            catch (Exception ex)
            {
                return new RegisterResult { Success = false, Message = "Ошибка при регистрации: " + ex.Message };
            }
        }
        public bool SubmitAuthorRequest(int userId, string reason)
        {
            var user = db.Users.Find(userId);
            if (user == null || user.HasAuthorRequest)
                return false;

            user.HasAuthorRequest = true;
            user.AuthorRequestStatus = "Pending";
            user.AuthorRequestReason = reason;
            user.AuthorRequestDate = DateTime.Now;

            db.SaveChanges();
            return true;
        }

        public List<User> GetAuthorRequests()
        {
            return db.Users
                     .Include(u => u.Role)
                     .Where(u => u.HasAuthorRequest && u.AuthorRequestStatus == "Pending")
                     .ToList();
        }

        public bool ProcessAuthorRequest(int userId, bool approve, string comment = "")
        {
            var user = db.Users.Find(userId);
            if (user == null || !user.HasAuthorRequest)
                return false;

            if (approve)
            {
                var authorRole = db.Roles.FirstOrDefault(r => r.Name == "Author");
                if (authorRole != null)
                {
                    user.RoleId = authorRole.Id;
                }
                user.AuthorRequestStatus = "Approved";
            }
            else
            {
                user.AuthorRequestStatus = "Rejected";
            }

            db.SaveChanges();
            return true;
        }
        public bool ChangeUserRole(int userId, string newRoleName)
        {
            var user = db.Users.Find(userId);
            if (user == null) return false;

            var newRole = db.Roles.FirstOrDefault(r => r.Name == newRoleName);
            if (newRole == null) return false;

            user.RoleId = newRole.Id;
            db.SaveChanges();
            return true;
        }
        public bool FreezeBook(int bookId, string reason = "Заморожено администратором")
        {
            var book = db.Books.Find(bookId);
            if (book == null) return false;

            book.IsFrozen = true;
            book.FreezeReason = reason;
            db.SaveChanges();
            return true;
        }

        public bool UnfreezeBook(int bookId)
        {
            var book = db.Books.Find(bookId);
            if (book == null) return false;

            book.IsFrozen = false;
            book.FreezeReason = null;
            db.SaveChanges();
            return true;
        }

        public List<Book> GetFrozenBooks()
        {
            return db.Books
                     .Include(b => b.Author)
                     .Where(b => b.IsFrozen)
                     .ToList();
        }
        // === ОТЗЫВЫ ===
        public List<Review> GetReviewsForBook(int bookId)
        {
            return db.Reviews
                     .Include(r => r.User)
                     .Where(r => r.BookId == bookId && !r.IsFrozen)
                     .OrderByDescending(r => r.CreatedAt)
                     .ToList();
        }

        public bool AddReview(Review review)
        {
            try
            {
                db.Reviews.Add(review);
                db.SaveChanges();

                // Пересчёт среднего рейтинга книги
                UpdateBookRating(review.BookId);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void UpdateBookRating(int bookId)
        {
            var reviews = db.Reviews
                            .Where(r => r.BookId == bookId && !r.IsFrozen)
                            .ToList();

            var book = db.Books.Find(bookId);

            if (book != null)
            {
                if (reviews.Any())
                {
                    double average = reviews.Average(r => r.Rating);
                    book.Rating = (float)Math.Round(average, 1);   // ← явное приведение
                }
                else
                {
                    book.Rating = 0.0f;   // ← явно float
                }

                db.SaveChanges();
            }
        }
        // ================== ЗАМОРОЗКА ПОЛЬЗОВАТЕЛЕЙ ==================
        public bool FreezeUser(int userId, string reason)
        {
            var user = db.Users.Find(userId);
            if (user == null) return false;

            user.IsFrozen = true;
            user.FreezeReason = reason;
            db.SaveChanges();
            return true;
        }

        public bool UnfreezeUser(int userId)
        {
            var user = db.Users.Find(userId);
            if (user == null) return false;

            user.IsFrozen = false;
            user.FreezeReason = null;
            db.SaveChanges();
            return true;
        }

        public List<User> GetFrozenUsers()
        {
            return db.Users
                     .Include(u => u.Role)
                     .Where(u => u.IsFrozen)
                     .ToList();
        }
        public bool AddBookToShelf(int userId, int bookId, string statusName)
        {
            try
            {
                // Находим или создаём статус
                var status = db.ReadingStatuses.FirstOrDefault(s => s.Name == statusName);
                if (status == null)
                {
                    status = new ReadingStatus { Name = statusName };
                    db.ReadingStatuses.Add(status);
                    db.SaveChanges();
                }

                // Удаляем книгу из других списков пользователя
                var existing = db.UserBookLists.Where(ubl => ubl.UserId == userId && ubl.BookId == bookId);
                db.UserBookLists.RemoveRange(existing);

                // Добавляем в новый список
                db.UserBookLists.Add(new UserBookList
                {
                    UserId = userId,
                    BookId = bookId,
                    StatusId = status.Id,
                    AddedAt = DateTime.Now
                });

                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        // ================== ЖАЛОБЫ ==================
        public bool AddComplaint(int userId, int bookId)
        {
            try
            {
                var complaint = new Complaint
                {
                    UserId = userId,
                    BookId = bookId,
                    Reason = "Жалоба от пользователя",   // стандартная причина
                    CreatedAt = DateTime.Now
                };

                db.Complaints.Add(complaint);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Complaint> GetAllComplaints()
        {
            return db.Complaints
                     .Include(c => c.User)
                     .Include(c => c.Book)
                     .Where(c => !c.IsResolved)
                     .OrderByDescending(c => c.CreatedAt)
                     .ToList();
        }

        public bool ResolveComplaint(int complaintId)
        {
            var complaint = db.Complaints.Find(complaintId);
            if (complaint == null) return false;

            complaint.IsResolved = true;
            db.SaveChanges();
            return true;
        }
    }
}