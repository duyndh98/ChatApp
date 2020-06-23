using CyDu.Model;
using CyDu.Ultis;
using CyDu.Windown;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

namespace CyDu
{
    /// <summary>
    /// Interaction logic for LoginWindown.xaml
    /// </summary>
    public partial class LoginWindown : Window
    {
        public LoginWindown()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {

            LoginAsync().Wait();
            if (AppInstance.getInstance().GetUser().Id != 0)
            {

                MainChatWindown main = new MainChatWindown();
                main.Show();
                this.Close();
            }

            //MainChatWindown main = new MainChatWindown();
            //main.Show();
            //this.Close();
        }

        private async Task LoginAsync()
        {
            string username = usernameBox.Text;
            string pass = PasswordBox.Password;
            LoginUser userLogin = new LoginUser() { username = username, password = pass };

            string url = Ultils.getUrl();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response =  client.PostAsJsonAsync("api/Users/Authenticate", userLogin).Result;
                if (response.IsSuccessStatusCode)
                {
                    User user = await response.Content.ReadAsAsync<User>();
                    AppInstance.getInstance().SetUser(user);
                }
                else
                {
                    _404Mess mess = await response.Content.ReadAsAsync<_404Mess>();
                    ErrLable.Content = mess.Message;
                }
            }
        }
    }
}
