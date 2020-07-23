using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CyDu.Model
{
    public class Resource
    {
        [JsonProperty("id",NullValueHandling =NullValueHandling.Ignore)]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
