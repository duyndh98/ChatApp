using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CyDu.ViewModel
{
    public class ContactListItem /*: INotifyPropertyChanged*/
    { 
        public String Username { get; set; }
        public long UserId { get; set; }
        public long Status { get; set; }
        public bool IsSelected { get; set; }

        //public event PropertyChangedEventHandler PropertyChanged;

        //public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
    }
}
