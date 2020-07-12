using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
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
using Newtonsoft.Json;

namespace CyDu.Dialogs
{
    /// <summary>
    /// Interaction logic for ConversationMembersDialog.xaml
    /// </summary>
    public partial class ConversationMembersDialog : Window
    {
        ObservableCollection<ConversationMemberItem> Users;
        private long ConversationId;
        private long ConversationHost;

        public ConversationMembersDialog(long conversationId)
        {
            Users = new ObservableCollection<ConversationMemberItem>();
            this.ConversationId = conversationId;
            InitializeComponent();
            LoadUserList();
        }

        private void LoadUserList()
        {
            Users = new ObservableCollection<ConversationMemberItem>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Ultils.url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.GetAsync("/api/Conversations/"+ ConversationId, HttpCompletionOption.ResponseContentRead).Result;
                if (response.IsSuccessStatusCode)
                {
                    Conversation conversation = response.Content.ReadAsAsync<Conversation>().Result;
                    ConversationHost = conversation.HostUserId;
                }
            }

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Ultils.url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.GetAsync("/api/Conversations/Members?id=" + ConversationId, HttpCompletionOption.ResponseContentRead).Result;
                //HttpResponseMessage response = client.PostAsJsonAsync("/api/ConversationsView/Members", AppInstance.getInstance().getUser().Id).Result;
                List<User> users = response.Content.ReadAsAsync<List<User>>().Result;
                foreach (User Conv_users in users)
                {
                    ConversationMemberItem mem = new ConversationMemberItem()
                    {
                        FullName = Conv_users.Username,
                        Id = Conv_users.Id,
                        isAdmin = false,
                        Role = Conv_users.Role,
                        Username = Conv_users.Username,
                        AllowDelete = true,
                        Avatar = ImageSupportInstance.getInstance().GetUserImageFromId(Conv_users.Id)
                    };
                    //if (mem.Id == AppInstance.getInstance().GetUser().Id)
                    //{
                    //    mem.AllowDelete = false;
                    //}
                    if (mem.Id != ConversationHost)
                    {
                        mem.AllowToapprove = true;
                    }
                    Users.Add(mem);
                }
            }
            lvUser.ItemsSource = Users;
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        private void deleteuser_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ConversationMemberItem user = button.DataContext as ConversationMemberItem;
            string content = "Kick user "+user.FullName;


            if (user.Id == AppInstance.getInstance().GetUser().Id)
            {
                content = "Out this conversation ";
            }
            ConfirmDialog dialog = new ConfirmDialog(content);
            bool? result = dialog.ShowDialog();
           
            if (result==true)
            {
                if (user.Id == ConversationHost)
                {
                    System.Windows.MessageBox.Show("Host cannot out the conversation");
                    return;
                }

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Ultils.url + "/api/Conversations/Members");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);

                    HttpResponseMessage response = client.DeleteAsync("/api/Conversations/Members/" + ConversationId + "/" + user.Id).Result;
                    if (response.IsSuccessStatusCode)
                    {

                    }
                    else
                    {
                        _404Mess mess = response.Content.ReadAsAsync<_404Mess>().Result;
                        System.Windows.MessageBox.Show(mess.Message);

                    }
                }


            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ConversationMemberItem user = button.DataContext as ConversationMemberItem;
            ConfirmDialog dialog = new ConfirmDialog("Approve user " + user.FullName +" become host of coversation");
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Ultils.url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                    ConversationMember memberinconversation = new ConversationMember()
                    {
                        UserIds = user.Id
                    };
                    HttpResponseMessage response = client.PutAsJsonAsync("/api/Conversations/HostMember/" + ConversationId, memberinconversation).Result;                
                    if (response.IsSuccessStatusCode)
                    {
                        LoadUserList();
                    }
                    else
                    {
                        _404Mess err = response.Content.ReadAsAsync<_404Mess>().Result;
                        System.Windows.MessageBox.Show(err.Message);
                    }
                }
            }
        }
    }
}
