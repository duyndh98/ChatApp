using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CyDu.Model
{
    class MessageWithTimespan
    {
        [JsonProperty("item1")]
        public long Item1 { get; set; }

        [JsonProperty("item2")]
        public Message[] Item2 { get; set; }
    }
}
