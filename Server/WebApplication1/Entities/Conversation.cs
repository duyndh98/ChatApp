using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Entities
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public int HostUserId { get; set; }

        public virtual ICollection<ConversationUser> ConversationUsers { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}
