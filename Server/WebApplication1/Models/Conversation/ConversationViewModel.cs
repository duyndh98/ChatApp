using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Entities;

namespace WebApplication1.Models
{
    public class ConversationViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int HostUserId { get; set; }
    }
}
