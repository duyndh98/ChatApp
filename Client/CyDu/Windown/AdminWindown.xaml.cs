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

namespace CyDu.Windown
{
    /// <summary>
    /// Interaction logic for AdminWindown.xaml
    /// </summary>
    public partial class AdminWindown : Window
    {
        public ObservableCollection<AdminPanelItem> Items { get; set; }

        public AdminWindown()
        {
                getUsersAsync().Wait();
            //Items = LoadData();
            InitializeComponent();
            lvAdmin.ItemsSource = Items;
        }

        public ObservableCollection<AdminPanelItem> LoadData()
        {
            ObservableCollection<AdminPanelItem> items = new ObservableCollection<AdminPanelItem>()
            {
                new AdminPanelItem()
                {
                    Id = "0",
                    Fullname = "FUll name",
                    Name = "NAme",
                    isAdmin = true
                },
                new AdminPanelItem()
                {
                    Id = "1",
                    Fullname = "name is fulled",
                    Name = "Nem",
                    isAdmin = false
                },
                new AdminPanelItem()
                {
                    Id = "1",
                    Fullname = "name is fulled",
                    Name = "Nem",
                    isAdmin = false
                },
                new AdminPanelItem()
                {
                    Id = "1",
                    Fullname = "name is fulled",
                    Name = "Nem",
                    isAdmin = false
                },
                new AdminPanelItem()
                {
                    Id = "1",
                    Fullname = "name is fulled",
                    Name = "Nem",
                    isAdmin = false
                },
                new AdminPanelItem()
                {
                    Id = "1",
                    Fullname = "name is fulled",
                    Name = "Nem",
                    isAdmin = false
                },
                new AdminPanelItem()
                {
                    Id = "1",
                    Fullname = "name is fulled",
                    Name = "Nem",
                    isAdmin = false
                },
                new AdminPanelItem()
                {
                    Id = "1",
                    Fullname = "name is fulled",
                    Name = "Nem",
                    isAdmin = false
                },
                new AdminPanelItem()
                {
                    Id = "1",
                    Fullname = "name is fulled",
                    Name = "Nem",
                    isAdmin = false
                },
                new AdminPanelItem()
                {
                    Id = "1",
                    Fullname = "name is fulled",
                    Name = "Nem",
                    isAdmin = false
                },
                new AdminPanelItem()
                {
                    Id = "1",
                    Fullname = "name is fulled",
                    Name = "Nem",
                    isAdmin = false
                }
            };
            return items;
        }

        private async Task getUsersAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Ultils.getUrl());
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.GetAsync("/api/Users", HttpCompletionOption.ResponseContentRead).Result;
                if (response.IsSuccessStatusCode)
                {
                    List<User> users = await response.Content.ReadAsAsync<List<User>>();
                    Items = new ObservableCollection<AdminPanelItem>();
                    foreach (User user in users)
                    {
                        bool isAdmin = false;
                        if (user.Role.Equals("Admin"))
                        {
                            isAdmin = true;
                        }
                        Items.Add(new AdminPanelItem() { 
                            Fullname = user.FullName,
                            Id = user.Id.ToString(),
                            Name = user.Username,
                            isAdmin = isAdmin
                        });
                    }
                }
            }
        }

        private void savebt_Click(object sender, RoutedEventArgs e)
        {
            ItemCollection collection = lvAdmin.Items;
            for (int i = 0; i < collection.Count; i++)
            {
                AdminPanelItem user = collection.GetItemAt(i) as AdminPanelItem;
                string id = user.Id;
                Role role = new Role();
                role.role = "User";
                if (user.isAdmin)
                {
                    role.role = "Admin";
                }
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Ultils.getUrl());
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                    HttpResponseMessage response = client.PutAsJsonAsync("/api/Users/Grant/" + id, role).Result;
                }
            }
            Close();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
