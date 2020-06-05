using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Entities;

namespace WebApplication1.Models
{
    public class MessageCreationModel
    {
        public DateTime ArrivalTime { get; set; }
        public MessageType Type { get; set; }
        public string Content { get; set; }

        public int ConversationId { get; set; }
        public int SenderId { get; set; }
    }
}
