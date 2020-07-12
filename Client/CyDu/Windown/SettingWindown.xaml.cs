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
using System.Windows.Shapes;
using CyDu.Ultis;
using Microsoft.Win32;

namespace CyDu.Windown
{
    /// <summary>
    /// Interaction logic for SettingWindown.xaml
    /// </summary>
    public partial class SettingWindown : Window
    {
        public SettingWindown()
        {
            InitializeComponent();
            if (!AppInstance.getInstance().GetUser().Role.Equals("Admin"))
            {
                AdminPanel.IsEnabled = false;
            }
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
         ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            string isstartup = rk.GetValue("CyDu") as string;
            if (isstartup != null)
            {
                if (isstartup.Count() > 0)
                {
                    toggle.IsChecked = true;
                }
                else
                    toggle.IsChecked = false;
            }
        }

        private void AdminPanel_Click(object sender, RoutedEventArgs e)
        {
            AdminWindown windown = new AdminWindown();
            windown.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            windown.Show();
        }

        private void btclose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
           ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rk.SetValue("CyDu", System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
         ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rk.DeleteValue("CyDu", false);
        }

        private void btlogout_Click(object sender, RoutedEventArgs e)
        {
            //LoginWindown windown = new LoginWindown();
            //windown.Show();
            this.DialogResult = true;
            this.Close();
        }
    }
}
