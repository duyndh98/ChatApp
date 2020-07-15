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
            History = _listhistory;
            InitializeComponent();
            this.DataContext = this;
            lvHistory.ItemsSource = History;

            ConversationWorker = new BackgroundWorker();
            ConversationWorker.DoWork += ConversationWorker_DoWork;
            ConversationWorker.RunWorkerCompleted += ConversationWorker_RunWorkerCompleted;
            ConversationWorker.RunWorkerAsync(6000);
        }

        private void ConversationWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ObservableCollection<ConversationsView> conversationviews = e.Result as ObservableCollection<ConversationsView>;
          
            bool isNew = false;
            if (conversationviews.Count == History.Count)
            {
                foreach (var i in conversationviews)
                {
                   ConversationsView cv = History.Where(x => x.Text == i.Text).FirstOrDefault();
                    if (cv ==null)
                    {
                        isNew = true;
                        break;
                    }
                    cv = History.Where(x => x.Mess == i.Mess).FirstOrDefault();
                    if (cv == null)
                    {
                        if (History.Count != 0 && i.MessUserId != AppInstance.getInstance().GetUser().Id )//lúc start ko bold và là ng r ko bold
                            i.fontWeight = "Normal"; //bold nhé
                        else
                            i.fontWeight = "Normal";
                        isNew = true;
                        break;
                    }
                 
                }
            }
            else
                isNew = true;
            
            if (isNew/* && !SearchText.Equals("")*/) // và ko phải là đang search
            {
                History = conversationviews;
                lvHistory.ItemsSource = History;
            }

            ConversationWorker.RunWorkerAsync(6000);

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
                ConversationList =  response.Content.ReadAsAsync<List<Conversation>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    AppInstance.getInstance().SetConversation(ConversationList);
                }
            }

            foreach (Conversation conver in ConversationList)
            {
                string lastMess = "";
                string time = "";
                long lastMessUserId = 0;
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
                    lastMessUserId = mess.SenderId;
                }

                conversationsViewsList.Add(new ConversationsView()
                {
                    Pk_seq = conver.Id,
                    Text = conver.Name,
                    Mess = lastMess,
                    MessUserId = lastMessUserId,
                    Date = time,
                    Avatar = ImageSupportInstance.getInstance().GetUserImageFromId(lastMessUserId)
                }); ;
            }
            e.Result = conversationsViewsList;
        }

        public void ApplySearching(string searchText)
        {
            ObservableCollection<ConversationsView> result = new ObservableCollection<ConversationsView>();
            foreach (var item in History)
            {
                if (item.Text.Contains(searchText))
                {
                    result.Add(item);
                }
            }
            lvHistory.ItemsSource = result;
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
                History.ElementAt(lv.SelectedIndex).fontWeight = null;;

                HistoryEventHandler(this, new HistoryItemSelectedArgs()
                {
                    itemIndex = lv.SelectedIndex,
                    pk_seq = History.ElementAt(lv.SelectedIndex).Pk_seq,
                    text = History.ElementAt(lv.SelectedIndex).Text
                });

                lvHistory.ItemsSource = History;

            }
            catch (ArgumentOutOfRangeException)
            {

            }
        
          
        }

        private void FrameworkElement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void cbBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selected = cbBox.SelectedIndex;
            List<ConversationsView> his;
            if (selected==0)
            {
               his =  History.OrderByDescending(x => x.Date).ToList();
            }
            else
            {
                 his = History.OrderByDescending(x => x.MessUserId).ToList();

            }
            lvHistory.ItemsSource = his;
        }
    }

    public class HistoryItemSelectedArgs : EventArgs
    {
        public int itemIndex { get; set; }
        public long pk_seq { get; set; }
        public string text { get; set; }
    }
}
