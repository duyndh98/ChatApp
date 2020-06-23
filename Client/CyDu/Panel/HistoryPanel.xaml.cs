using CyDu.Dialogs;
using CyDu.Model;
using CyDu.Ultis;
using CyDu.ViewModel;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for HistoryWindown.xaml
    /// </summary>
    public partial class HistoryWindown : UserControl
    {
        public ObservableCollection<ConversationsView> History { get; set; }
        public event EventHandler HistoryEventHandler;
        private BackgroundWorker ConversationWorker;

        public HistoryWindown(ObservableCollection<ConversationsView> _listhistory)
        {
            InitializeComponent();
            History = _listhistory;
            lvHistory.ItemsSource = History;

            ConversationWorker = new BackgroundWorker();
            ConversationWorker.DoWork += ConversationWorker_DoWork;
            ConversationWorker.RunWorkerCompleted += ConversationWorker_RunWorkerCompleted;
            ConversationWorker.RunWorkerAsync(2000);
        }

        private void ConversationWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ObservableCollection<ConversationsView> conversationviews = e.Result as ObservableCollection<ConversationsView>;
            History = conversationviews;
            lvHistory.ItemsSource = History;
            ConversationWorker.RunWorkerAsync(2000);


        }

        private void ConversationWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string url = Ultils.getUrl();
            List<Conversation> ConversationList = new List<Conversation>();
            ObservableCollection<ConversationsView> conversationsViewsList = new ObservableCollection<ConversationsView>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.GetAsync("/api/Users/Owner/Conversations", HttpCompletionOption.ResponseContentRead).Result;
                //HttpResponseMessage response = client.PostAsJsonAsync("/api/ConversationsView/Members", AppInstance.getInstance().getUser().Id).Result;
                ConversationList =  response.Content.ReadAsAsync<List<Conversation>>().Result;
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
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                    HttpResponseMessage response = client.GetAsync("/api/Conversations/Members?id=" + conversationId, HttpCompletionOption.ResponseContentRead).Result;
                    //HttpResponseMessage response = client.PostAsJsonAsync("/api/ConversationsView/Members", AppInstance.getInstance().getUser().Id).Result;
                    List<User> users = response.Content.ReadAsAsync<List<User>>().Result;
                    foreach (User Conv_users in users)
                    {
                        if (Conv_users.Id != AppInstance.getInstance().GetUser().Id)
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
                        User users = response.Content.ReadAsAsync<User>().Result;
                        user += users.Username + " ";
                        AppInstance.getInstance().SetFullname(users.Id, users.FullName);
                    }
                }
                if (user.Length > 20)
                {
                    user = user.Substring(0, 17) + "...";
                }

                conversationsViewsList.Add(new ConversationsView()
                {
                    Pk_seq = conver.Id,
                    Username = user
                }); ;
            }
            e.Result = conversationsViewsList;
        }


        public void Refresh()
        {

        }

        private void Btadd_Click(object sender, RoutedEventArgs e)
        {
            AddChatDialog dialog = new AddChatDialog();
            dialog.ShowDialog();
            
        }

        private void lvHistory_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListView lv = sender as ListView;
            try
            {
                HistoryEventHandler(this, new HistoryItemSelectedArgs()
                {
                    itemIndex = lv.SelectedIndex,
                    pk_seq = History.ElementAt(lv.SelectedIndex).Pk_seq,
                    username = History.ElementAt(lv.SelectedIndex).Username
                });
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        
          
        }

        private void FrameworkElement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }

    public class HistoryItemSelectedArgs : EventArgs
    {
        public int itemIndex { get; set; }
        public long pk_seq { get; set; }
        public string username { get; set; }
    }
}
