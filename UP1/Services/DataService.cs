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

        public DataService()
        {
            SeedTestData();   // Принудительно создаём тестовых пользователей
        }

        private void SeedTestData()
        {
            if (!db.Users.Any())
            {
                db.Roles.Add(new Role { Name = "Administrator" });
                db.Roles.Add(new Role { Name = "Author" });
                db.Roles.Add(new Role { Name = "User" });
                db.SaveChanges();

                var adminRole = db.Roles.First(r => r.Name == "Administrator");
                var authorRole = db.Roles.First(r => r.Name == "Author");
                var userRole = db.Roles.First(r => r.Name == "User");

                db.Users.Add(new User
                {
                    Login = "admin",
                    PasswordHash = "admin_password",
                    DisplayName = "Администратор",
                    Email = "admin@mail.com",
                    RoleId = adminRole.Id
                });

                db.Users.Add(new User
                {
                    Login = "author",
                    PasswordHash = "author_password",
                    DisplayName = "Автор",
                    Email = "author@mail.com",
                    RoleId = authorRole.Id
                });

                db.Users.Add(new User
                {
                    Login = "user",
                    PasswordHash = "user_password",
                    DisplayName = "Пользователь",
                    Email = "user@mail.com",
                    RoleId = userRole.Id
                });

                db.SaveChanges();
            }
        }

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

        internal IEnumerable<object> GetAllUsers()
        {
            throw new NotImplementedException();
        }
    }
}