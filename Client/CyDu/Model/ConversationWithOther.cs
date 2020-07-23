using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CyDu.Model
{
    class ConversationWithOther
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("userIds")]
        public long[] UserIds { get; set; }
    }
}
