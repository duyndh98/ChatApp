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
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using CyDu.Model;

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
        public event EventHandler NotifiEventHandler;
        public event EventHandler SearchEventHandler;
        public string User { get; set; }
        public int NotifiBadge { get; set; }
        public BitmapImage Useravatar { get; set; }

        private BackgroundWorker notifiupdateWorker;
        public MenuControl()
        {
            User = "Cylasion";
            NotifiBadge = 0;
            InitializeComponent();
            this.DataContext = this;
           
            userFullName.Content = AppInstance.getInstance().GetUser().FullName;

            string basestr = ImageSupportInstance.getInstance().getUserBase64Image(AppInstance.getInstance().GetUser().Id);
            if (!basestr.Equals(""))
            {
                Useravatar = ImageSupportInstance.getInstance().ConvertFromBaseString(basestr);
            }

            notifiupdateWorker = new BackgroundWorker();
            notifiupdateWorker.DoWork += NotifiupdateWorker_DoWork;
            notifiupdateWorker.RunWorkerCompleted += NotifiupdateWorker_RunWorkerCompleted;
            notifiupdateWorker.RunWorkerAsync();
        }

        private void NotifiupdateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string str = e.Result as string;
            int badge = int.Parse(str);
            if (badge != NotifiBadge)
            {
                NotifiBadge = int.Parse(str);
                notifibadge.Badge = NotifiBadge;
            }  
            notifiupdateWorker.RunWorkerAsync(2000);

        }

        private void NotifiupdateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            long notifi = 0;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Ultils.url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.GetAsync("/api/Users/Owner/Contacts", HttpCompletionOption.ResponseContentRead).Result;
                if (response.IsSuccessStatusCode)
                {
                    notifi = response.Content.ReadAsAsync<List<Contact>>().Result.Where(x=>x.Status==0).ToList().Count;
                }
            }
            e.Result =  notifi.ToString();
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

        private void btNotifi_Click(object sender, RoutedEventArgs e)
        {
            NotifiEventHandler(this, new EventArgs());
        }
        private void AdminPanel_Click(object sender, RoutedEventArgs e)
        {
            SettingWindown windown = new SettingWindown();
            windown.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            bool? result = windown.ShowDialog();
            if (result==true)
            {
                LogoutEventHandler(this, new EventArgs());
            }
        }

        private void UserIconClick(object sender, RoutedEventArgs e)
        {
            popup_acc.PlacementTarget = btUserIcon;
            popup_acc.Placement = PlacementMode.Right;
            popup_acc.HorizontalOffset = -5;
            popup_acc.IsOpen = !popup_acc.IsOpen;

            userinfopopup.LogoutEventhandler += Userinfopopup_LogoutEventhandler;
            userinfopopup.UpdateIconEventhandler += Userinfopopup_UpdateIconEventhandler;
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

        private void Userinfopopup_UpdateIconEventhandler(object sender, EventArgs e)
        {
            string basestr = ImageSupportInstance.getInstance().getUserBase64Image(AppInstance.getInstance().GetUser().Id);
            Useravatar = ImageSupportInstance.getInstance().ConvertFromBaseString(basestr);
            userimg.Source = Useravatar;
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
