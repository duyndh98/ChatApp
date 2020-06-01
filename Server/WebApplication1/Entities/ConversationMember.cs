using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Entities
{
    public class ConversationUser
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ConversationId { get; set; }

        public virtual User User { get; set; }
        public virtual Conversation Conversation { get; set; }
    }
}
