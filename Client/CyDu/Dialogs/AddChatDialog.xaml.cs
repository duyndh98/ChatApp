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
    /// Interaction logic for AddChatDialog.xaml
    /// </summary>
    public partial class AddChatDialog : Window
    {
        ObservableCollection<ContactListItem> Contacts;

        public AddChatDialog()
        {
            InitializeComponent();
            Contacts = new ObservableCollection<ContactListItem>();
            List<Contact> contacts = AppInstance.getInstance().GetContacts();
            foreach (Contact contact in contacts)
            {
                long id = contact.FromUserId;
                if (id == AppInstance.getInstance().GetUser().Id)
                {
                    id = contact.ToUserId;
                }
                Contacts.Add(new ContactListItem()
                {
                    FromUserId = contact.FromUserId,
                    ToUserId = contact.ToUserId,
                    Status = contact.Status,
                    Username = AppInstance.getInstance().GetFullname(id),
                    IsSelected = false,
                    Avatar = ImageSupportInstance.getInstance().GetUserImageFromId(id)
                }); ; ;
            }
            lvContact.ItemsSource = Contacts;
        }

        private void btcancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private  void btOk_Click(object sender, RoutedEventArgs e)
        {
            AddConversationWithOtherUser();
        }

        private async void AddConversationWithOtherUser()
        {
            ContactListItem item = lvContact.SelectedItem as ContactListItem;

            long id = item.FromUserId;
            if (id==AppInstance.getInstance().GetUser().Id)
            {
                id = item.ToUserId;
            }
            string name = "Conversation with " + AppInstance.getInstance().GetFullname(id);
            if (!conversationnametb.Text.Equals(""))
            {
                name = conversationnametb.Text;
            }

            ConversationWithOther conver = new ConversationWithOther();
            conver.Name = name;
            conver.UserIds = new long[] { id };
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Ultils.getUrl());
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.PostAsJsonAsync("/api/Conversations/WithMembers", conver).Result;
                if (response.IsSuccessStatusCode)
                {
                    this.DialogResult = true;

                }
                else
                {
                    _404Mess mess = await response.Content.ReadAsAsync<_404Mess>();
                    errlb.Text = mess.Message;

                }
            }
        }

      
    }
}
