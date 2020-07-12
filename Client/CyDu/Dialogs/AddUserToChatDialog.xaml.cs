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
using CyDu.Model;
using CyDu.Ultis;
using CyDu.ViewModel;

namespace CyDu.Dialogs
{
    /// <summary>
    /// Interaction logic for AddUserToChatDialog.xaml
    /// </summary>
    public partial class AddUserToChatDialog : Window
    {
        ObservableCollection<ContactListItem> Contacts;

        private long ConversationId;
        public AddUserToChatDialog(long conversationid)
        {
            InitializeComponent();
            this.ConversationId = conversationid;

            Contacts = new ObservableCollection<ContactListItem>();
            List<Contact> contacts = AppInstance.getInstance().GetContacts();
            foreach (Contact contact in contacts )
            {
                long id = contact.FromUserId;
                if (id == AppInstance.getInstance().GetUser().Id)
                {
                    id = contact.ToUserId;
                }
                Contacts.Add(new ContactListItem()
                {
                    ToUserId = contact.ToUserId,
                    Status = contact.Status,
                    FromUserId = contact.FromUserId,
                    Username = AppInstance.getInstance().GetFullname(id),
                    IsSelected = false,
                    Avatar = ImageSupportInstance.getInstance().GetUserImageFromId(id)
                });; ;
            }
            lvContact.ItemsSource = Contacts;

        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in lvContact.SelectedItems)
            {
                ContactListItem contact = item as ContactListItem;
                long userid = contact.ToUserId;
                if (contact.ToUserId == AppInstance.getInstance().GetUser().Id) 
                {
                    userid = contact.FromUserId;
                }
                MemberConversation mem_conver = new MemberConversation()
                {
                    ConversationId = ConversationId,
                    UserId = userid
                };
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Ultils.url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                    HttpResponseMessage response = client.PostAsJsonAsync("/api/Conversations/Members", mem_conver).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        this.DialogResult = true;

                    }
                    else
                    {
                        _404Mess mess = response.Content.ReadAsAsync<_404Mess>().Result;
                        errlb.Text = mess.Message;
                    }
                }

                
            }
           
        }

        private async Task AddMembertoConversationAsync()
        {
            //string id = tbInput.Text;
            //if (id.Equals(""))
            //{
            //    return;
            //}
            //string url = Ultils.getUrl();
            //MemberConversation mem_conver = new MemberConversation()
            //{
            //    ConversationId = ConversationId,
            //    UserId = long.Parse(id)
            //};
            

            //using (HttpClient client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(url);
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
            //    HttpResponseMessage response = client.PostAsJsonAsync("/api/Conversations/Members", mem_conver).Result;
            //    if (response.IsSuccessStatusCode)
            //    {
            //        this.DialogResult = true;

            //    }
            //    else
            //    {
            //        _404Mess mess = await response.Content.ReadAsAsync<_404Mess>();
            //        errlb.Text = mess.Message;
            //        //this.DialogResult = false;

            //        // errlb.Text = mess.Message;
            //    }
            //}
        }


        private void btcancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;

        }

        private void lvContact_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //ListView lv = sender as ListView;
            //try
            //{
            //    ContactListItem lvselected = lv.SelectedItem as ContactListItem;
            //    ContactListItem item = Contacts.FirstOrDefault(i => i.Username == lvselected.Username);
            //    item.IsSelected = true;
            //}
            //catch (ArgumentOutOfRangeException)
            //{

            //}

        }
    }
}
