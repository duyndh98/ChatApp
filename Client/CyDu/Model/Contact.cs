using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CyDu.Model
{
    public class Contact
    {
        [JsonProperty("fromUserId", NullValueHandling = NullValueHandling.Ignore)]
        public long FromUserId { get; set; }

        [JsonProperty("toUserId")]
        public long ToUserId { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public long Status { get; set; }
    }
}
