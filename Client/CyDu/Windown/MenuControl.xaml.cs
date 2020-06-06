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

namespace CyDu.Windown
{
    /// <summary>
    /// Interaction logic for MenuControl.xaml
    /// </summary>
    public partial class MenuControl : UserControl
    {
        public MenuControl()
        {
            InitializeComponent();
            userFullName.Content = AppInstance.getInstance().getUser().FullName;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btMess_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btCall_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btCOntact_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
