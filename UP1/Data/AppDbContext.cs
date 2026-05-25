using System.Data.Entity;
using UP1.Models;

namespace UP1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("ReadWriteDbConnection")
        {
            // При каждом запуске пересоздаём базу (для разработки)
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<AppDbContext>());
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<UserBookList> UserBookLists { get; set; }
        public DbSet<ReadingStatus> ReadingStatuses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookGenre>()
                .HasKey(bg => new { bg.BookId, bg.GenreId });

            // Отключаем каскадное удаление, чтобы избежать циклов
            modelBuilder.Entity<Review>()
                .HasRequired(r => r.Book)
                .WithMany(b => b.Reviews)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Review>()
                .HasRequired(r => r.User)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserBookList>()
                .HasRequired(ubl => ubl.Book)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserBookList>()
                .HasRequired(ubl => ubl.User)
                .WithMany()
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}