using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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
using Microsoft.Win32;

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
        public ChattingPanel(long pk_seq)
        {
            this.ConversationId = pk_seq;

            InitializeComponent();
            this.DataContext = this;
            lasttimespan = 0;

            //Thread thread = new Thread(UpdatePanel);
            //thread.Name = "LoadMessThread";
            //thread.Start();
            //LoadMessageAsync();
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWorkAsync;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync(3000);
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
                        MessageBox messBos = new MessageBox(title, mess.Content, mess.ArrivalTime.ToString("hh:mm:ss dd-MM-yyyy"), side);
                        mainPanel.Children.Add(messBos);
                    }
                    if (mess.Type == 2)
                    {
                        MessageImageBox messBox = new MessageImageBox(title, mess.Content, mess.ArrivalTime.ToString("hh:mm:ss dd-MM-yyyy"), (MessageImageBox.Side) side);
                        mainPanel.Children.Add(messBox);
                    }
                   
                }
            }
            
            worker.RunWorkerAsync(3000);
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

        //public async void LoadMessageAsync()
        //{
        //    string url = Ultils.getUrl();
         
        //    mainPanel.Children.Clear();
        //    using (HttpClient client = new HttpClient())
        //        {
        //        client.BaseAddress = new Uri(url);
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
        //        HttpResponseMessage response = client.GetAsync("/api/Conversations/Messages/New?id=" + this.ConversationId+ "&lastTimeSpan="+lasttimespan, HttpCompletionOption.ResponseContentRead).Result;
        //        //HttpResponseMessage response = client.PostAsJsonAsync("/api/ConversationsView/Members", AppInstance.getInstance().getUser().Id).Result;
        //        if (response.IsSuccessStatusCode)
        //        {
        //            MessageWithTimespan listmassage = await response.Content.ReadAsAsync<MessageWithTimespan>();
        //            lasttimespan = 0;
        //            //lasttimespan = listmassage.Item1;
        //            foreach (Message mess in listmassage.Item2)
        //            {
        //                MessageBox.Side side;
        //                if (mess.SenderId == AppInstance.getInstance().GetUser().Id)
        //                {
        //                    side = MessageBox.Side.User;
        //                }
        //                else
        //                {
        //                    side = MessageBox.Side.Other;
        //                }
        //                string title = AppInstance.getInstance().GetFullname(mess.SenderId);
        //                MessageBox messBos = new MessageBox(title, mess.Content, mess.ArrivalTime.ToString("hh:mm:ss dd-MM-yyyy"), side);
        //                mainPanel.Children.Add(messBos);
        //            }
        //        }
        //    }

        //    using (HttpClient client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(url);
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
        //        HttpResponseMessage response = client.GetAsync("/api/Conversations/" + this.ConversationId , HttpCompletionOption.ResponseContentRead).Result;
        //        if (response.IsSuccessStatusCode)
        //        {
        //            Conversation conver = response.Content.ReadAsAsync<Conversation>().Result;
        //        }

        //    }
        //    Titlelable.Content = Title;
        //    mainScroll.ScrollToBottom();
        //}

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
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
                //LoadMessageAsync();
            }
        }

        private void sendImagebtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Image file (*.png)|*png";
            if (openFileDialog.ShowDialog() == true)
            {
                SendImage(openFileDialog.FileNames[0]);
            }
        }

        private void SendImage(string path)
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            int length = Convert.ToInt32(file.Length);
            byte[] data = new byte[length];
            file.Read(data, 0, length);
            file.Close();
            string base64 = Convert.ToBase64String(data);


            MessageSend mess = new MessageSend()
            {
                Content = base64,
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
            //LoadMessageAsync();
        }

        private void btMemList_Click(object sender, RoutedEventArgs e)
        {
            ConversationMembersDialog dialog = new ConversationMembersDialog(ConversationId);
            bool? result = dialog.ShowDialog();
        }
    }
}
