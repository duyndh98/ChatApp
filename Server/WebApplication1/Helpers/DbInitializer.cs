using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Services;

namespace WebApplication1.Helpers
{
    public static class DbInitializer
    {
        public static void Seed(WebApplication1Context context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
                return;

            byte[] passwordHash, passwordSalt;

            // Seed admin
            UserService.CreatePasswordHash("admin@pw", out passwordHash, out passwordSalt);
            context.Users.Add(new User
            {
                UserName = "admin",
                FullName = "Administrator",
                Role = Role.Admin,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });

            // Seed user
            UserService.CreatePasswordHash("guest@pw", out passwordHash, out passwordSalt);
            context.Users.Add(new User
            {
                UserName = "guest",
                FullName = "Guest",
                Role = Role.User,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });

            context.SaveChanges();
        }
    }
}
