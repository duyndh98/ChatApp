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
using CyDu.Windown;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace CyDu.Panel
{
    /// <summary>
    /// Interaction logic for MenuControl.xaml
    /// </summary>
    public partial class MenuControl : UserControl
    {
        public event EventHandler HistoryEventHandler;
        public event EventHandler LogoutEventHandler;
        public event EventHandler ContactEventHandler;
        public event EventHandler SearchEventHandler;
        public string User { get; set; }
        public int NotifiBadge { get; set; }
        public MenuControl()
        {
            User = "Cylasion";
            NotifiBadge = 5;
            InitializeComponent();
            this.DataContext = this;
            if (!AppInstance.getInstance().GetUser().Role.Equals("Admin"))
            {
                AdminPanel.Visibility = Visibility.Hidden;
            }
            userFullName.Content = AppInstance.getInstance().GetUser().FullName;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btMess_Click(object sender, RoutedEventArgs e)
        {
            HistoryEventHandler(this, new EventArgs());
        }

        private void btCall_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btCOntact_Click(object sender, RoutedEventArgs e)
        {
            ContactEventHandler(this, new EventArgs());
        }

        private void AdminPanel_Click(object sender, RoutedEventArgs e)
        {
            AdminWindown windown = new AdminWindown();
            windown.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            windown.Show();
        }

        private void UserIconClick(object sender, RoutedEventArgs e)
        {
            popup_acc.PlacementTarget = btUserIcon;
            popup_acc.Placement = PlacementMode.Right;
            popup_acc.HorizontalOffset = -5;
            popup_acc.IsOpen = !popup_acc.IsOpen;

            userinfopopup.LogoutEventhandler += Userinfopopup_LogoutEventhandler;
            //popup_acc.IsOpen = true;
            //DispatcherTimer timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromSeconds(5);
            //timer.Start();
            //timer.Tick += delegate
            //{
            //    popup_acc.Visibility = Visibility.Collapsed;
            //    popup_acc.IsOpen = false;
            //};
        }

        private void Userinfopopup_LogoutEventhandler(object sender, EventArgs e)
        {
            LogoutEventHandler(this, new EventArgs());
        }

        private void PopupMouseLeave(object sender, MouseEventArgs e)
        {
            //popup_acc.Visibility = Visibility.Collapsed;
            //popup_acc.IsOpen = false;
        }

        private void SearchTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchEventHandler(this, new SearchTextChangeEventArgs() { Text = SearchTb.Text }) ;
        }


        public class SearchTextChangeEventArgs : EventArgs
        {
            public string Text { get; set; }
        }
    }
}
