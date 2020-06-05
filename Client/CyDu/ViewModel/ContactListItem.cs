using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyDu.ViewModel
{
    public class ContactListItem
    {
        public String Username { get; set; }

        public Char FirsChar { get; set; }

        private void NameOnchange()
        {
            this.FirsChar = Username[0];
        }
    }
}
