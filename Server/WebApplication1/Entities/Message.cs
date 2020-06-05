using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Entities
{
    public enum MessageType
    {
        Text,
        Image
    }

    public class Message
    {
        [Key]
        public int Id { get; set; }
        public DateTime ArrivalTime { get; set; }
        public MessageType Type { get; set; }
        public string Content { get; set; }

        public int ConversationId { get; set; }
        public virtual Conversation Conversation { get; set; }

        public int SenderId { get; set; }
        public virtual User Sender { get; set; }
    }
}
