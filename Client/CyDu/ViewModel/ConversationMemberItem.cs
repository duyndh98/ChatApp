using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace CyDu.ViewModel
{
    class ConversationMemberItem
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }
        public bool AllowDelete { get; set; }
        public bool isAdmin { get; set; }
        public bool AllowToapprove { get; set; }

        public BitmapImage Avatar { get; set; }
    }
}
