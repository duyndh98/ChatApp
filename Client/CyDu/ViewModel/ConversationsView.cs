using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CyDu.ViewModel
{
    public class ConversationsView 
    {
        public BitmapImage Avatar { get; set; }
        public String Text { get; set; }
        public long Pk_seq { get; set; }
        public String Mess { get; set; }
        public String Date { get; set; }
        public long MessUserId { get; set; }
        public string fontWeight { get; set; }
    }
}
