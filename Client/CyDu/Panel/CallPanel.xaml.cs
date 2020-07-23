using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using CyDu.Model;
using CyDu.Ultis;
using CyDu.ViewModel;
using CyDu.Windown;
using Microsoft.AspNetCore.SignalR.Client;

namespace CyDu.Panel
{
    /// <summary>
    /// Interaction logic for CallPanel.xaml
    /// </summary>
    public partial class CallPanel : UserControl
    {
        HubConnection connection;
        ObservableCollection<CallPanelItem> CallList ;

        public CallPanel()
        {
            CallList = new ObservableCollection<CallPanelItem>();
            InitializeComponent();
            SetupHubConnectionAsync();
        }

        private void lvCall_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListView lv = sender as ListView;
            try
            {
                CallPanelItem item = CallList.ElementAt(lv.SelectedIndex);

                long userid = item.Userid;
                long caonverid = item.Cvid;

                int videotype = 1;
                if (userid !=AppInstance.getInstance().GetUser().Id)
                {
                    videotype = 2;
                }
                VideocallWindown windown = new VideocallWindown(caonverid, videotype);
                windown.Show();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private async Task SetupHubConnectionAsync()
        {
            string url = Ultils.url + "chathub";
            connection = new HubConnectionBuilder()
              .WithUrl(url)
              .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };


            connection.On<string, string>("ReceiveMessage", (type, message) =>
            {
                
                if (type.Equals("call"))
                {
                    long cvid = long.Parse(message.Split('-')[0]);

                    Conversation cv = AppInstance.getInstance().GetConversations().Where(x => x.Id == cvid).First();
                    if (cv != null)
                    {
                        long userid = long.Parse(message.Split('-')[1]);
                        CallList.Add(new CallPanelItem() { 
                            Cvid = cvid,
                            Name = cv.Name+" has a Calling ",
                            Userid = userid
                        });
                    }
                    lvCall.ItemsSource = CallList;
                }

            });

            try
            {
                await connection.StartAsync();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}
