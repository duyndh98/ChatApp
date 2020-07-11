using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Entities;

namespace WebApplication1.Data
{
    public class WebApplication1Context : DbContext
    {
        public WebApplication1Context(DbContextOptions<WebApplication1Context> options)
            : base(options)
        {
        }

        public DbSet<WebApplication1.Entities.User> Users { get; set; }

        public DbSet<WebApplication1.Entities.Conversation> Conversations { get; set; }

        public DbSet<WebApplication1.Entities.ConversationUser> ConversationUsers { get; set; }

        public DbSet<WebApplication1.Entities.Message> Messages { get; set; }

        public DbSet<WebApplication1.Entities.Contact> Contacts { get; set; }

        public DbSet<WebApplication1.Entities.Resource> Resources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConversationUser>().HasKey(x => new { x.ConversationId, x.UserId });

            modelBuilder.Entity<Contact>().HasKey(x => new { x.FromUserId, x.ToUserId });
        }
    }
}
