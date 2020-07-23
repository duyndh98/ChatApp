using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.AspNetCore.SignalR.Client;

namespace CyDu.Windown
{
    /// <summary>
    /// Interaction logic for ChatHub.xaml
    /// </summary>
    public partial class ChatHub : Window
    {
        HubConnection connection;

        public ChatHub()
        {
            InitializeComponent();
           
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:44344/chathub")
                .WithAutomaticReconnect()
                .Build();
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
            connection.Reconnecting += error =>
            {
                Debug.Assert(connection.State == HubConnectionState.Reconnecting);

                contentLable.Text = "Reconnection "+ new Random().Next(0, 5);

                return Task.CompletedTask;
            };
        }

        private async void ConnectBt_Click(object sender, RoutedEventArgs e)
        {
            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{user}: {message}";
                    contentLable.Text = newMessage;
                });
            });

            try
            {
                await connection.StartAsync();
                contentLable.Text = "CONNECTION START";
                ConnectBt.IsEnabled = false;
                SendBt.IsEnabled = true;
            }
            catch (Exception ex)
            {
                contentLable.Text = ex.Message;
            }
        }

        private async void SendBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await connection.InvokeAsync("SendMessage",
                  new Random().Next(0, 5)+ " user ", new Random().Next(0, 5) + " text ");
            }
            catch (Exception ex)
            {
                contentLable.Text = ex.Message;
            }
        }
    }
}
