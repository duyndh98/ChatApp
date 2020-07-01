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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CyDu.Model;
using CyDu.Ultis;
using CyDu.ViewModel;

namespace CyDu.Dialogs
{
    /// <summary>
    /// Interaction logic for ConversationMembersDialog.xaml
    /// </summary>
    public partial class ConversationMembersDialog : Window
    {
        ObservableCollection<User> Users;
        private long ConversationId;

        public ConversationMembersDialog(long conversationId)
        {
            Users = new ObservableCollection<User>();
            this.ConversationId = conversationId;
            InitializeComponent();
            LoadUserList();
        }

        private void LoadUserList()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Ultils.url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.GetAsync("/api/Conversations/Members?id=" + ConversationId, HttpCompletionOption.ResponseContentRead).Result;
                //HttpResponseMessage response = client.PostAsJsonAsync("/api/ConversationsView/Members", AppInstance.getInstance().getUser().Id).Result;
                List<User> users =  response.Content.ReadAsAsync<List<User>>().Result;
                foreach (User Conv_users in users)
                {
                    Users.Add(Conv_users);
                }
            }
            lvUser.ItemsSource = Users;
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }
    }
}
