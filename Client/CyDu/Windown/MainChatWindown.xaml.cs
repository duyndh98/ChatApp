using CyDu.Model;
using CyDu.Ultis;
using CyDu.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

namespace CyDu.Windown
{
    /// <summary>
    /// Interaction logic for MainChatWindown.xaml
    /// </summary>
    public partial class MainChatWindown : Window
    {
        public ObservableCollection<ConversationsView> ConversationViews;
        public MainChatWindown()
        {
           
            InitializeComponent();
            ConversationViews = new ObservableCollection<ConversationsView>();


            Menupanel.HistoryEventHandler += Window_HistoryPannelEventHandle;

            //load conversation panel first
            LoadConversation().Wait();
            HistoryWindown Historywindown = new HistoryWindown(ConversationViews);
            Historywindown.Name = "Historywd";
            Historywindown.HistoryEventHandler += HistoryItemEventHandler;
            LeftPanel.Children.Clear();
            LeftPanel.Children.Add(Historywindown);


            RightPanel.Children.Add(new WelcomePanel());

        }


        private void Window_HistoryPannelEventHandle(object sender, EventArgs e)
        {
            LoadConversation().Wait();
            HistoryWindown Historywindown = new HistoryWindown(ConversationViews);
            Historywindown.Name = "Historywd";
            Historywindown.HistoryEventHandler += HistoryItemEventHandler;
            LeftPanel.Children.Clear();
            LeftPanel.Children.Add(Historywindown);

        }

        private void HistoryItemEventHandler(object sender, EventArgs e)
        {
            HistoryItemSelectedArgs itemindex = e as HistoryItemSelectedArgs;
            long pkseq = itemindex.pk_seq;
            string usernames = itemindex.username;
            RightPanel.Children.Clear();
            ChattingPanel chattingPanel = new ChattingPanel();
            RightPanel.Children.Add(chattingPanel);
            chattingPanel.LoadMessageAsync(pkseq, usernames).Wait(); ;


        }

        private void setChatpanel(object sender , HistoryItemSelectedArgs e )
        {
        }

        //=======================================

        private async Task LoadConversation()
        {


            string url = Ultils.getUrl();
            List<Conversation> ConversationList = new List<Conversation>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.GetAsync("/api/Users/JoinedConversations", HttpCompletionOption.ResponseContentRead).Result;
                //HttpResponseMessage response = client.PostAsJsonAsync("/api/ConversationsView/Members", AppInstance.getInstance().getUser().Id).Result;
                ConversationList = await response.Content.ReadAsAsync<List<Conversation>>();
                if (response.IsSuccessStatusCode)
                {
                    AppInstance.getInstance().SetConversation(ConversationList);        
                }
            }

            foreach (Conversation conver in ConversationList)
            {
                List<long> userIds = new List<long>();
                string user = "";
                long conversationId = conver.ConversationId;
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                    HttpResponseMessage response = client.GetAsync("/api/Conversations/Members?id="+conversationId, HttpCompletionOption.ResponseContentRead).Result;
                    //HttpResponseMessage response = client.PostAsJsonAsync("/api/ConversationsView/Members", AppInstance.getInstance().getUser().Id).Result;
                    ConversationList = await response.Content.ReadAsAsync<List<Conversation>>();
                    foreach (Conversation Conv_users in ConversationList)
                    {
                        userIds.Add(Conv_users.UserId);
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
                        HttpResponseMessage response = client.GetAsync("/api/Users/"+id, HttpCompletionOption.ResponseContentRead).Result;
                        User users = await response.Content.ReadAsAsync<User>();
                        user += users.Username + " ";
                        AppInstance.getInstance().setFullname(users.Id, users.FullName);
                    }
                }
                if (user.Length>20)
                {
                    user = user.Substring(0, 17) + "...";
                }

                ConversationViews.Add(new ConversationsView()
                {
                    Pk_seq = conver.ConversationId,
                    Username = user
                }); ;
            }
        }
    }
}
