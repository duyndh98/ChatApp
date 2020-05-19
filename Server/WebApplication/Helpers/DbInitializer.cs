using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Entities;
using WebApplication.Services;

namespace WebApplication.Helpers
{
    public static class DbInitializer
    {
        public static void Seed(WebApplicationContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
                return;

            byte[] passwordHash, passwordSalt;

            // Seed admin
            UserService.CreatePasswordHash("admin@pw", out passwordHash, out passwordSalt);
            context.Users.Add(new User
            {
                Username = "admin",
                FullName = "Administrator",
                Role = Role.Admin,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });

            // Seed user
            UserService.CreatePasswordHash("guest@pw", out passwordHash, out passwordSalt);
            context.Users.Add(new User
            {
                Username = "guest",
                FullName = "Guest",
                Role = Role.User,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });

            context.SaveChanges();
        }
    }
}
