using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CyDu.Model
{
    class _404Mess
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
