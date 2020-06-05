using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CyDu.Model
{
    /*
     
         {"id":1,"username":"admin","fullName":"Administrator","role":"Admin","token":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjEiLCJyb2xlIjoiQWRtaW4iLCJuYmYiOjE1OTEyODQ2MDEsImV4cCI6MTU5MTg4OTQwMSwiaWF0IjoxNTkxMjg0NjAxfQ.lSc5Q3XP8SxhznUDvVL2RFCr3PMVISEX6vv8UV7D2tY"}
         */
    public class User
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
