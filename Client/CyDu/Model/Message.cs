using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CyDu.Model
{
    class Message
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("arrivalTime")]
        public DateTimeOffset ArrivalTime { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("conversationId")]
        public long ConversationId { get; set; }

        [JsonProperty("senderId")]
        public long SenderId { get; set; }
    }
}

