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
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("hostUserId" , NullValueHandling = NullValueHandling.Ignore)]
        public long HostUserId { get; set; }
    }

    public class ConversationMember
    {
        [JsonProperty("userId")]
        public long UserIds { get; set; }
    }
}
