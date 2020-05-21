using Microsoft.EntityFrameworkCore;
using WebApplication.Entities;

namespace WebApplication.Data
{
    public class WebApplicationContext : DbContext
    {
        public WebApplicationContext(DbContextOptions<WebApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
