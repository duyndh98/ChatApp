using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CyDu.Model
{
    public class Conversation
    {
        [JsonProperty("conversationId")]
        public long ConversationId { get; set; }

        [JsonProperty("userId")]
        public long UserId { get; set; }
    }
}
