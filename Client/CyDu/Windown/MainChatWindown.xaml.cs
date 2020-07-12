using CyDu.Model;
using CyDu.Panel;
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
using static CyDu.Panel.MenuControl;
using static CyDu.Windown.ContactListControl;

namespace CyDu.Windown
{
    /// <summary>
    /// Interaction logic for MainChatWindown.xaml
    /// </summary>
    public partial class MainChatWindown : Window
    {
        public ObservableCollection<ConversationsView> ConversationViews;
        public ObservableCollection<ContactListItem> ContactViews;

        private HistoryWindown Historywindown;
        private ContactListControl ContactWindown;
        private NotifiPanel NotifiWindown;

        public MainChatWindown()
        {
           
            InitializeComponent();
            
            ConversationViews = new ObservableCollection<ConversationsView>();
            ContactViews = new ObservableCollection<ContactListItem>();


            MenuControl Menupanel = new MenuControl();

            Menupanel.HistoryEventHandler += Window_HistoryPannelEventHandle;
            Menupanel.LogoutEventHandler += Menupanel_LogoutEventHandler;
            Menupanel.ContactEventHandler += Menupanel_ContactEventHandler;
            Menupanel.NotifiEventHandler += Menupanel_NotifiEventHandler;
            Menupanel.SearchEventHandler += Menupanel_SearchEventHandler;
            TopLeftPanel.Children.Add(Menupanel);

            //load contact
            LoadContact().Wait();
            ContactWindown = new ContactListControl(ContactViews);
            ContactWindown.ContactEvenHandle += ContactItemEventHandler;

            //load notifi
            NotifiWindown = new NotifiPanel(ContactViews);

            //load conversation panel first
            LoadConversation().Wait();
            Historywindown = new HistoryWindown(ConversationViews);
            Historywindown.Name = "Historywd";
            Historywindown.HistoryEventHandler += HistoryItemEventHandler;
            
            BotLeftPanel.Children.Clear();
            BotLeftPanel.Children.Add(Historywindown);


            RightPanel.Children.Add(new WelcomePanel());

            //AdminWindown admin = new AdminWindown();
            //admin.Show();
        }

        private void Menupanel_NotifiEventHandler(object sender, EventArgs e)
        {
            ContactViews.Clear();
            LoadContact().Wait();
            NotifiWindown.Name = "Notifi";
            BotLeftPanel.Children.Clear();
            BotLeftPanel.Children.Add(NotifiWindown);
        }

        private void Menupanel_SearchEventHandler(object sender, EventArgs e)
        {
            SearchTextChangeEventArgs arg = e as SearchTextChangeEventArgs;
            Historywindown.ApplySearching(arg.Text);
            ContactWindown.ApplySearching(arg.Text);
        }

        private void Menupanel_ContactEventHandler(object sender, EventArgs e)
        {
            ContactViews.Clear();
            LoadContact().Wait();
            ContactWindown.Name = "Contact";
            BotLeftPanel.Children.Clear();
            BotLeftPanel.Children.Add(ContactWindown);

        }

        private void Menupanel_LogoutEventHandler(object sender, EventArgs e)
        {
            LoginWindown windown = new LoginWindown();
            windown.Show();
            this.Close();
        }

        private void Window_HistoryPannelEventHandle(object sender, EventArgs e)
        {
            ConversationViews.Clear();
            LoadConversation().Wait();
            Historywindown.Name = "Historywd";
            Historywindown.HistoryEventHandler += HistoryItemEventHandler;
            BotLeftPanel.Children.Clear();
            BotLeftPanel.Children.Add(Historywindown);

        }

        private void HistoryItemEventHandler(object sender, EventArgs e)
        {
            HistoryItemSelectedArgs itemindex = e as HistoryItemSelectedArgs;
            long pkseq = itemindex.pk_seq;
            RightPanel.Children.Clear();
            ChattingPanel chattingPanel = new ChattingPanel(pkseq);
            RightPanel.Children.Add(chattingPanel);
            //chattingPanel.LoadMessageAsync().Wait(); ;
        }

        private void ContactItemEventHandler(object sender, EventArgs e)
        {
            ContactItemSelectedArgs item = e as ContactItemSelectedArgs;
            ConversationWithOther conver = new ConversationWithOther();
            conver.Name = "Noname conversation";
            conver.UserIds = new long[] { item.userId };
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Ultils.getUrl());
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.PostAsJsonAsync("/api/Conversations/WithMembers", conver).Result;
                if (response.StatusCode==System.Net.HttpStatusCode.Conflict || response.StatusCode == System.Net.HttpStatusCode.OK)//trùng hoặc vừa tạo mới
                {
                    Conversation conversation = response.Content.ReadAsAsync<Conversation>().Result;
     
                    RightPanel.Children.Clear();
                    ChattingPanel chattingPanel = new ChattingPanel(conversation.Id);
                    RightPanel.Children.Add(chattingPanel);

                }
                else
                {
               
                }
            }
        }
        //================================================================================

        private async Task LoadContact()
        {
            string url = Ultils.getUrl();
            List<Contact> contactList = new List<Contact>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.GetAsync("/api/Users/Owner/Contacts", HttpCompletionOption.ResponseContentRead).Result;
                contactList = await response.Content.ReadAsAsync<List<Contact>>();
                if (response.IsSuccessStatusCode)
                {
                    AppInstance.getInstance().SetContacts(contactList);
                }
                
            }

            foreach (Contact contact in contactList)
            {
                
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                   
                    HttpResponseMessage response = client.GetAsync("/api/Users/" + contact.FromUserId, HttpCompletionOption.ResponseContentRead).Result;
                    User users = await response.Content.ReadAsAsync<User>();
                    AppInstance.getInstance().SetFullname(users.Id, users.FullName);

                    response = client.GetAsync("/api/Users/" + contact.ToUserId, HttpCompletionOption.ResponseContentRead).Result;
                    users = await response.Content.ReadAsAsync<User>();
                    AppInstance.getInstance().SetFullname(users.Id, users.FullName);

                    long id = contact.FromUserId;
                    if (id == AppInstance.getInstance().GetUser().Id)
                    {
                        id = contact.ToUserId;
                    }

                    ContactViews.Add(new ContactListItem()
                    {
                        ToUserId = contact.ToUserId,
                        Status = contact.Status,
                        FromUserId = contact.FromUserId,
                        Avatar = ImageSupportInstance.getInstance().GetUserImageFromId(id)
                    }) ;
                }
            }

        }

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
                HttpResponseMessage response = client.GetAsync("/api/Users/Owner/Conversations", HttpCompletionOption.ResponseContentRead).Result;
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
                long conversationId = conver.Id;

                //Đoạn này load tên user lên có thể bỏ qua
                //using (HttpClient client = new HttpClient())
                //{
                //    client.BaseAddress = new Uri(url);
                //    client.DefaultRequestHeaders.Accept.Clear();
                //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                //    HttpResponseMessage response = client.GetAsync("/api/Conversations/Members?id=" + conversationId, HttpCompletionOption.ResponseContentRead).Result;
                //    HttpResponseMessage response = client.PostAsJsonAsync("/api/ConversationsView/Members", AppInstance.getInstance().getUser().Id).Result;
                //    List<User> users = await response.Content.ReadAsAsync<List<User>>();
                //    foreach (User Conv_users in users)
                //    {
                //        if (Conv_users.Id != AppInstance.getInstance().GetUser().Id)
                //            userIds.Add(Conv_users.Id);
                //    }
                //}

                //foreach (long id in userIds)
                //{
                //    using (HttpClient client = new HttpClient())
                //    {
                //        client.BaseAddress = new Uri(url);
                //        client.DefaultRequestHeaders.Accept.Clear();
                //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                //        HttpResponseMessage response = client.GetAsync("/api/Users/" + id, HttpCompletionOption.ResponseContentRead).Result;
                //        User users = await response.Content.ReadAsAsync<User>();
                //        user += users.Username + " ";
                //        AppInstance.getInstance().SetFullname(users.Id, users.FullName);
                //    }
                //}
                //if (user.Length > 20)
                //{
                //    user = user.Substring(0, 17) + "...";
                //}
                string lastMess = "";
                string time = "";
                using (HttpClient client = new HttpClient())
                {
                   
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                    HttpResponseMessage response = client.GetAsync("/api/Conversations/Messages/Last?id="+conver.Id, HttpCompletionOption.ResponseContentRead).Result;
                    Message mess = response.Content.ReadAsAsync<Message>().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        lastMess = mess.Content;
                    }

                    if (mess.Type==2)
                    {
                        lastMess = "Send a picture";
                    }
                    time = mess.ArrivalTime.ToString("hh:mm dd/MM");
                    if (time.Equals("12:00 01/01"))
                    {
                        time = "";
                    }
                }

                ConversationViews.Add(new ConversationsView()
                {
                    Pk_seq = conver.Id,
                    Text = conver.Name,
                    Mess = lastMess,
                    Date = time,
                    fontWeight = "Normal"
                }); 
            }
        }
    }
}
