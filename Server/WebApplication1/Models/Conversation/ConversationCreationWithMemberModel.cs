using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class ConversationCreationWithMemberModel
    {
        public string Name { get; set; }
        public ICollection<int> UserIds { get; set; }
    }
}
