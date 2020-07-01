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
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Conversation2
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
