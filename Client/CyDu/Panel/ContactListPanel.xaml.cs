using CyDu.Dialogs;
using CyDu.Model;
using CyDu.Ultis;
using CyDu.ViewModel;
using Microsoft.AspNetCore.SignalR.Client;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CyDu.Windown
{
    /// <summary>
    /// Interaction logic for ContactListControl.xaml
    /// </summary>
    /// 


    public partial class ContactListControl : UserControl
    {
        public ObservableCollection<ContactListItem> Constacts { get; set; }
        private BackgroundWorker Bgworker;
        public EventHandler ContactEvenHandle;

        public ContactListControl(ObservableCollection<ContactListItem> _Constacts)
        {
            InitializeComponent();
            Constacts = new ObservableCollection<ContactListItem>();
            foreach (var item in _Constacts )
            {
                if (item.Status==1)
                {
                    long id = item.FromUserId;
                    if (id== AppInstance.getInstance().GetUser().Id)
                    {
                        id = item.ToUserId;
                    }
                    item.Username = AppInstance.getInstance().GetFullname(id);

                    Constacts.Add(item);
                }
            }
            lvContact.ItemsSource = this.Constacts;
            Bgworker = new BackgroundWorker();
            Bgworker.DoWork += Bgworker_DoWork;
            Bgworker.RunWorkerCompleted += Bgworker_RunWorkerCompleted;
            Bgworker.RunWorkerAsync(2000);
        }

     

        private void Bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ObservableCollection<ContactListItem> ContactListViews = e.Result as ObservableCollection<ContactListItem>;
            if (ContactListViews.Count != Constacts.Count)
            {
                Constacts = ContactListViews;
                lvContact.ItemsSource = Constacts;
            }
            Bgworker.RunWorkerAsync(6000);
        }

        private void Bgworker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Contact> contactList = new List<Contact>();
            ObservableCollection<ContactListItem> contactItems = new ObservableCollection<ContactListItem>();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Ultils.url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.GetAsync("/api/Users/Owner/Contacts", HttpCompletionOption.ResponseContentRead).Result;
                if (response.IsSuccessStatusCode)
                {
                    contactList = response.Content.ReadAsAsync<List<Contact>>().Result;
                }
            }

            foreach (Contact contact in contactList)
            {
                if (contact.Status==1)
                {
                    long id = contact.FromUserId;
                    if (id == AppInstance.getInstance().GetUser().Id)
                    {
                        id = contact.ToUserId;
                    }

                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(Ultils.url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                        HttpResponseMessage response = client.GetAsync("/api/Users/" + id, HttpCompletionOption.ResponseContentRead).Result;
                        User users = response.Content.ReadAsAsync<User>().Result;
                        AppInstance.getInstance().SetFullname(users.Id, users.FullName);
                        contactItems.Add(new ContactListItem()
                        {
                            ToUserId = contact.ToUserId,
                            Username = AppInstance.getInstance().GetFullname(id),
                            Status = 1,
                            FromUserId = contact.FromUserId,
                            Avatar = ImageSupportInstance.getInstance().GetUserImageFromId(id)
                        });;
                    }
                }
                
            }
            e.Result = contactItems;
        }

        public void ApplySearching(string searchText)
        {
            ObservableCollection<ContactListItem> result = new ObservableCollection<ContactListItem>();
            foreach (var item in Constacts)
            {
                if (item.Username.Contains(searchText))
                {
                    result.Add(item);
                }
            }
            lvContact.ItemsSource = result;
        }
        private void Btadd_Click(object sender, RoutedEventArgs e)
        {
            AddNewContactDialog dialog = new AddNewContactDialog();
            bool? result = dialog.ShowDialog();
            if (result== true)
            {
                List<Contact> contactList = new List<Contact>();
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Ultils.url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                    HttpResponseMessage response = client.GetAsync("/api/Users/Owner/Contacts", HttpCompletionOption.ResponseContentRead).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        contactList = response.Content.ReadAsAsync<List<Contact>>().Result;
                    }
                }
                AppInstance.getInstance().SetContacts(contactList);



                //send to hub
              
            }
        }

        private void lvContact_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListView lv = sender as ListView;
            try
            {  
                long id = Constacts.ElementAt(lv.SelectedIndex).FromUserId;
                if (id == AppInstance.getInstance().GetUser().Id)
                {
                    id = Constacts.ElementAt(lv.SelectedIndex).ToUserId;
                }
                ContactEvenHandle(this, new ContactItemSelectedArgs()
                {
                    userId = id
                }) ;

                lvContact.ItemsSource = Constacts;

            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        public class ContactItemSelectedArgs : EventArgs
        {
            public long userId;
        }
    }
}
