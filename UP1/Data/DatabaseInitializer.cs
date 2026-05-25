using System.Data.Entity;
using System.Linq;
using UP1.Models;

namespace UP1.Data
{
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<AppDbContext>
    {
        protected override void Seed(AppDbContext context)
        {
            // Роли
            if (!context.Roles.Any())
            {
                context.Roles.Add(new Role { Name = "Administrator" });
                context.Roles.Add(new Role { Name = "Author" });
                context.Roles.Add(new Role { Name = "User" });
                context.SaveChanges();
            }

            var adminRole = context.Roles.First(r => r.Name == "Administrator");
            var authorRole = context.Roles.First(r => r.Name == "Author");
            var userRole = context.Roles.First(r => r.Name == "User");

            // Пользователи
            if (!context.Users.Any())
            {
                context.Users.Add(new User
                {
                    Login = "admin",
                    PasswordHash = "admin_password",
                    DisplayName = "Администратор",
                    Email = "admin@mail.com",
                    RoleId = adminRole.Id
                });

                context.Users.Add(new User
                {
                    Login = "author",
                    PasswordHash = "author_password",
                    DisplayName = "Автор",
                    Email = "author@mail.com",
                    RoleId = authorRole.Id
                });

                context.Users.Add(new User
                {
                    Login = "user",
                    PasswordHash = "user_password",
                    DisplayName = "Пользователь",
                    Email = "user@mail.com",
                    RoleId = userRole.Id
                });

                context.SaveChanges();
            }

            base.Seed(context);
        }
    }
}