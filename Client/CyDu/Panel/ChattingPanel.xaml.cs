using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CyDu.Dialogs;
using CyDu.Model;
using CyDu.Panel;
using CyDu.Ultis;
using MaterialDesignThemes.Wpf;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace CyDu.Windown
{
    /// <summary>
    /// Interaction logic for ChattingPanel.xaml
    /// </summary>
    public partial class ChattingPanel : UserControl
    {
        public string ChatPannelTitle { get; set; }
        private long ConversationId;
        private string Title;
        private long lasttimespan;
        BackgroundWorker worker;
        HubConnection connection;

        public ChattingPanel(long pk_seq)
        {
            this.ConversationId = pk_seq;

            InitializeComponent();
            this.DataContext = this;
            lasttimespan = 0;

            //Background worker update mess
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWorkAsync;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync(0);

            SetupHubConnectionAsync();
        }

        private async Task SetupHubConnectionAsync()
        {
            connection = new HubConnectionBuilder()
              .WithUrl("https://localhost:44344/chathub")
              .Build();
            
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
        

            connection.On<string, string>("ReceiveMessage", (type, message) =>
            {
                if (type.Equals("mess"))
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ReceiveMessage(message);
                    });
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

        private void ReceiveMessage(string message)
        {
            Message mess = JsonConvert.DeserializeObject<Message>(message);
            if (mess.ConversationId!=ConversationId)
            {
                return;
            }
            MessageBox.Side side;
            if (mess.SenderId == AppInstance.getInstance().GetUser().Id)
            {
                side = MessageBox.Side.User;
            }
            else
            {
                side = MessageBox.Side.Other;
            }
            string title = AppInstance.getInstance().GetFullname(mess.SenderId);

            if (mess.Type == 1)
            {
                MessageBox messBos = new MessageBox(title, mess.Content, mess.ArrivalTime.ToString("hh:mm:ss dd-MM-yyyy"),mess.SenderId,side);
                mainPanel.Children.Add(messBos);
            }
            if (mess.Type == 2)
            {
                MessageImageBox messBox = new MessageImageBox(title, mess.Content, mess.ArrivalTime.ToString("hh:mm:ss dd-MM-yyyy"), mess.SenderId, (MessageImageBox.Side)side);
                mainPanel.Children.Add(messBox);
            }

            mainScroll.ScrollToBottom();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Titlelable.Content = Title;
            mainPanel.Children.Clear();
            Message[] listmess = e.Result as Message[];
            if (listmess!=null)
            {
                foreach (Message mess in listmess)
                {
                    MessageBox.Side side;
                    if (mess.SenderId == AppInstance.getInstance().GetUser().Id)
                    {
                        side = MessageBox.Side.User;
                    }
                    else
                    {
                        side = MessageBox.Side.Other;
                    }
                    string title = AppInstance.getInstance().GetFullname(mess.SenderId);

                    if (mess.Type == 1)
                    {
                        MessageBox messBos = new MessageBox(title, mess.Content, mess.ArrivalTime.ToString("hh:mm:ss dd-MM-yyyy"),mess.SenderId, side);
                        mainPanel.Children.Add(messBos);
                    }
                    if (mess.Type == 2)
                    {
                        MessageImageBox messBox = new MessageImageBox(title, mess.Content, mess.ArrivalTime.ToString("hh:mm:ss dd-MM-yyyy"), mess.SenderId, (MessageImageBox.Side) side);
                        mainPanel.Children.Add(messBox);
                    }
                   
                }
            }
            mainScroll.ScrollToBottom();
            //worker.RunWorkerAsync(3000);
        }

        async void worker_DoWorkAsync(object sender, DoWorkEventArgs e)
        {

            string url = Ultils.getUrl();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.GetAsync("/api/Conversations/Messages/New?id=" + this.ConversationId + "&lastTimeSpan=" + lasttimespan, HttpCompletionOption.ResponseContentRead).Result;
                //HttpResponseMessage response = client.PostAsJsonAsync("/api/ConversationsView/Members", AppInstance.getInstance().getUser().Id).Result;
                MessageWithTimespan listmassage = await response.Content.ReadAsAsync<MessageWithTimespan>();
                if (response.IsSuccessStatusCode)
                {
                    //lasttimespan = listmassage.Item1;
                    lasttimespan = 0;
                    e.Result = listmassage.Item2;
                    
                }
            }
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.GetAsync("/api/Conversations/" + this.ConversationId, HttpCompletionOption.ResponseContentRead).Result;
                if (response.IsSuccessStatusCode)
                {
                    Conversation conver = response.Content.ReadAsAsync<Conversation>().Result;
                    this.Title = conver.Name;
                }

            }

            using (HttpClient client = new HttpClient())
            {
                
            }

        }

   
        private async void sendMessage()
        {
            string url = Ultils.getUrl();
            string content = messTextbox.Text;
            MessageSend mess = new MessageSend()
            {
                Content = content,
                ConversationId = this.ConversationId,
                Type = 1
            };
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.PostAsJsonAsync("/api/Messages", mess).Result;
                if (!response.IsSuccessStatusCode)
                {
                    System.Windows.MessageBox.Show("Lỗi khi gửi tin");
                }
            }
            messTextbox.Text = "";


            //send to hub
            Message hubmess = new Message()
            {
                ArrivalTime = DateTimeOffset.Now,
                Content = mess.Content,
                ConversationId = mess.ConversationId,
                Id = 0,
                SenderId = AppInstance.getInstance().GetUser().Id,
                Type = mess.Type
            };

            string json = JsonConvert.SerializeObject(hubmess);
            try
            {
                await connection.InvokeAsync("SendMessage", "mess", json);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                sendMessage();
                //LoadMessageAsync();
            }
        }

        private async void ReloadMember()
        {
            string url = Ultils.getUrl();
            List<long> userIds = new List<long>();
            string user = "";
            long conversationId = this.ConversationId;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.GetAsync("/api/Conversations/Members?id=" + conversationId, HttpCompletionOption.ResponseContentRead).Result;
                //HttpResponseMessage response = client.PostAsJsonAsync("/api/ConversationsView/Members", AppInstance.getInstance().getUser().Id).Result;
                List<User> users = await response.Content.ReadAsAsync<List<User>>();
                foreach (User Conv_users in users)
                {
                    if(Conv_users.Id != AppInstance.getInstance().GetUser().Id)
                        userIds.Add(Conv_users.Id);
                }
            }

            foreach (long id in userIds)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                    HttpResponseMessage response = client.GetAsync("/api/Users/" + id, HttpCompletionOption.ResponseContentRead).Result;
                    User users = await response.Content.ReadAsAsync<User>();
                    user += users.Username + " ";
                    AppInstance.getInstance().SetFullname(users.Id, users.FullName);
                }
            }
            if (user.Length > 20)
            {
                user = user.Substring(0, 17) + "...";
            }
            this.Title = user;
        }


        private void btAddMem_Click(object sender, RoutedEventArgs e)
        {
            AddUserToChatDialog dialog = new AddUserToChatDialog(this.ConversationId);
            bool? result = dialog.ShowDialog();
            if (result==true)
            {

            }
        }

        private void sendImagebtn_Click(object sender, RoutedEventArgs e)
        {
            string path = ImageSupportInstance.getInstance().OpenChooseImageDialogBox();
            if (!path.Equals(""))
            {
                SendImage(path);
            }
        }

        private async void SendImage(string path)
        {
            Resource res = ImageSupportInstance.getInstance().UploadImage(path,720,640);


            MessageSend mess = new MessageSend()
            {
                Content = res.Id.ToString(),
                ConversationId = this.ConversationId,
                Type = 2
            };
            using (HttpClient client = new HttpClient())
            {
                string url = Ultils.getUrl();
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.PostAsJsonAsync("/api/Messages", mess).Result;
                if (!response.IsSuccessStatusCode)
                {
                    System.Windows.MessageBox.Show("Lỗi khi gửi tin");
                }
            }
            messTextbox.Text = "";



            //send to hub
            Message hubmess = new Message()
            {
                ArrivalTime = DateTimeOffset.Now,
                Content = mess.Content,
                ConversationId = mess.ConversationId,
                Id = 0,
                SenderId = AppInstance.getInstance().GetUser().Id,
                Type = mess.Type
            };

            string json = JsonConvert.SerializeObject(hubmess);
            try
            {
                await connection.InvokeAsync("SendMessage", "mess", json);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void btMemList_Click(object sender, RoutedEventArgs e)
        {
            ConversationMembersDialog dialog = new ConversationMembersDialog(ConversationId);
            bool? result = dialog.ShowDialog();
        }

        private void sendbtn_Click(object sender, RoutedEventArgs e)
        {
            sendMessage();
        }

        private void editcsname_Click(object sender, RoutedEventArgs e)
        {
            EditConversationNameDialog dialog = new EditConversationNameDialog(this.Title,this.ConversationId);
            bool? result = dialog.ShowDialog();
            if (result== true)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Ultils.url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                    HttpResponseMessage response = client.GetAsync("/api/Conversations/" + this.ConversationId, HttpCompletionOption.ResponseContentRead).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        Conversation conver = response.Content.ReadAsAsync<Conversation>().Result;
                        this.Title = conver.Name;
                    }

                }
                Titlelable.Content = Title;

            }
        }
    }
}
