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

        public List<Book> GetBooksOnShelf(int userId, string statusName)
        {
            return db.Books.Include(b => b.Author).ToList();
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
                    PasswordHash = password,        // В будущем сделать хэширование!
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

    }
}