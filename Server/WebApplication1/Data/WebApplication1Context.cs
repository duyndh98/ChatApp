using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1.Data
{
    public class WebApplication1Context : DbContext
    {
        public WebApplication1Context (DbContextOptions<WebApplication1Context> options)
            : base(options)
        {
        }

        public DbSet<WebApplication1.Entities.User> Users { get; set; }

        public DbSet<WebApplication1.Entities.Conversation> Conversations { get; set; }

        public DbSet<WebApplication1.Entities.ConversationUser> ConversationUsers { get; set; }

        public DbSet<WebApplication1.Entities.Message> Messages { get; set; }
    }
}
