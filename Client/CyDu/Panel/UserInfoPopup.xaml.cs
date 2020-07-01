using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CyDu.Ultis;

namespace CyDu.Panel
{
    /// <summary>
    /// Interaction logic for UserInfoPopup.xaml
    /// </summary>
    public partial class UserInfoPopup : UserControl
    {

        public event EventHandler LogoutEventhandler;

        public UserInfoPopup()
        {
            InitializeComponent();
            lbuser.Text ="Username : "+ AppInstance.getInstance().GetUser().Username;
            lbfullname.Text = "Fullname : " + AppInstance.getInstance().GetUser().FullName;
            lbrole.Text = "Role : "+AppInstance.getInstance().GetUser().Role;
        }

        private void btlogout_Click(object sender, RoutedEventArgs e)
        {
            LogoutEventhandler(this, new EventArgs());
        }
    }
}
