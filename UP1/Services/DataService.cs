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

        internal IEnumerable<object> GetAllUsers()
        {
            throw new NotImplementedException();
        }
    }
}