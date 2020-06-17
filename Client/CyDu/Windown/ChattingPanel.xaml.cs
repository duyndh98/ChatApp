using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
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

namespace CyDu.Windown
{
    /// <summary>
    /// Interaction logic for ChattingPanel.xaml
    /// </summary>
    public partial class ChattingPanel : UserControl
    {
        public string ChatPannelTitle { get; set; }
        private long ConversationId;
        private string usernames;

        public ChattingPanel()
        {

            InitializeComponent();
            this.DataContext = this;
        
        }
        public async Task LoadMessageAsync(long pk_seq,string usernames)
        {
            string url = Ultils.getUrl();
            this.ConversationId = pk_seq;
            this.usernames = usernames;
            mainPanel.Children.Clear();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.GetAsync("/api/Messages/Conversations?conversationId=" + pk_seq, HttpCompletionOption.ResponseContentRead).Result;
                //HttpResponseMessage response = client.PostAsJsonAsync("/api/ConversationsView/Members", AppInstance.getInstance().getUser().Id).Result;
                List<Message> listmassage = await response.Content.ReadAsAsync<List<Message>>();
                if (response.IsSuccessStatusCode)
                {
                    foreach (Message mess in listmassage)
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
                        string title = AppInstance.getInstance().getFullname(mess.SenderId) +"      "+ mess.ArrivalTime.ToString("hh:mm:ss dd-MM-yyyy");
                        MessageBox messBos = new MessageBox(title,mess.Content,side);
                        mainPanel.Children.Add(messBos);
                    }
                }
            }


            Titlelable.Content = usernames;

            mainScroll.ScrollToBottom();
        }

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
                LoadMessageAsync(this.ConversationId, this.usernames).Wait();
            }
        }
    }
}
