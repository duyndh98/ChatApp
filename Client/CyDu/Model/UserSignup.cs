using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CyDu.Model
{
    class UserSignup
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("avatar")]
        public long Avatar { get; set; }
    }
}
