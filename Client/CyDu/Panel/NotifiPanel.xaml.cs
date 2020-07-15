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
using CyDu.Model;
using CyDu.Ultis;
using CyDu.ViewModel;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace CyDu.Panel
{
    /// <summary>
    /// Interaction logic for NotifiPanel.xaml
    /// </summary>
    public partial class NotifiPanel : UserControl
    {
        public ObservableCollection<ContactListItem> Constacts { get; set; }
        private BackgroundWorker Bgworker;
        HubConnection connection;


        public NotifiPanel(ObservableCollection<ContactListItem> _constacts)
        {
            InitializeComponent();
            Constacts = new ObservableCollection<ContactListItem>();
            foreach (var item in _constacts)
            {
                if (item.Status==0)
                {
                    item.Username = AppInstance.getInstance().GetFullname(item.FromUserId);
                    Constacts.Add(item);
                };
            };

            lvNotifi.ItemsSource = this.Constacts;
            Bgworker = new BackgroundWorker();
            Bgworker.DoWork += Bgworker_DoWork;
            Bgworker.RunWorkerCompleted += Bgworker_RunWorkerCompleted;
            Bgworker.RunWorkerAsync(0);

            SetupHubConnectionAsync();

        }


        private void Bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ObservableCollection<ContactListItem> ContactListViews = e.Result as ObservableCollection<ContactListItem>;
            if (ContactListViews.Count != Constacts.Count)
            {
                Constacts = ContactListViews;
                lvNotifi.ItemsSource = Constacts;
            }
            //Bgworker.RunWorkerAsync(6000);
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
                if (contact.Status==0)
                {
                    long id = contact.FromUserId;
                    if (id == AppInstance.getInstance().GetUser().Id) {
                        continue;
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
                            FromUserId = id,
                            Username = AppInstance.getInstance().GetFullname(contact.ToUserId),
                            Status = 0
                        });
                    }
                }

              
            }
            e.Result = contactItems;
        }


        private async Task SetupHubConnectionAsync()
        {
            String url = Ultils.url + "chathub";
            connection = new HubConnectionBuilder()
              .WithUrl(url)
              .Build();
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };


            connection.On<string, string>("ReceiveMessage", (type, message) =>
            {
                if (type.Equals("contact"))
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ReceiveContactAdd(message);
                    });
                }

            });

            try
            {
                await connection.StartAsync();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void ReceiveContactAdd(string contactstr)
        {
            Contact contact = JsonConvert.DeserializeObject<Contact>(contactstr);
            if (contact.ToUserId == AppInstance.getInstance().GetUser().Id)
            {
                long id = contact.FromUserId;
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Ultils.url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                    HttpResponseMessage response = client.GetAsync("/api/Users/" + id, HttpCompletionOption.ResponseContentRead).Result;
                    User users = response.Content.ReadAsAsync<User>().Result;
                    AppInstance.getInstance().SetFullname(users.Id, users.FullName);
                    
                    Constacts.Add(new ContactListItem() { 
                        FromUserId =contact.FromUserId,
                        Status = 0,
                        ToUserId = contact.ToUserId,
                        Username = users.FullName
                    });
                    lvNotifi.ItemsSource = Constacts;
                }
            }

        }

        private void Denybt_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ContactListItem contact = button.DataContext as ContactListItem;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Ultils.url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.DeleteAsync("/api/Contacts/" + contact.FromUserId).Result;
                if (response.IsSuccessStatusCode)
                {
                    Bgworker.RunWorkerAsync();
                }
                else
                {
                    _404Mess err = response.Content.ReadAsAsync<_404Mess>().Result;
                    System.Windows.MessageBox.Show(err.Message);
                }
            }
        }

        private void Acceptbt_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ContactListItem contact = button.DataContext as ContactListItem;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Ultils.url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.PutAsJsonAsync("/api/Contacts/" + contact.FromUserId, new Contact() { Status = 1 }).Result;
                if (response.IsSuccessStatusCode)
                {
                    Bgworker.RunWorkerAsync();
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
