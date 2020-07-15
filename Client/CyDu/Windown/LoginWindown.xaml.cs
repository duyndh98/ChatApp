using CyDu.Model;
using CyDu.Ultis;
using CyDu.Windown;
using Microsoft.Win32;
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
            ReadSavedUser();
        }

        public void SetAccount(string username, string password)
        {
            usernameBox.Text = username;
            PasswordBox.Password = password;
        }

        private void ReadSavedUser()
        {
            //RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE",true);
            //RegistryKey subkey =  rk.CreateSubKey("CyDu");
            //try
            //{
            //    string username = subkey.GetValue("username").ToString();
            //    string pass = subkey.GetValue("pass").ToString();
            //}
            //catch (Exception)
            //{

            //}
         
        }

        private void WritePassword()
        {
            //RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE",true);
            //RegistryKey subkey = rk.CreateSubKey("CyDu");
            //try
            //{
            //    string username = usernameBox.Text;
            //    string pass = PasswordBox.Password;
            //    subkey.SetValue("username",username);
            //    subkey.SetValue("pass", pass);
            //}
            //catch (Exception)
            //{

            //}
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            WritePassword();
            LoginAsync().Wait();
            if (AppInstance.getInstance().GetUser().Id != 0)
            {

                MainChatWindown main = new MainChatWindown();
                main.Show();

                //ChatHub hub = new ChatHub();
                //hub.Show();

                Close();
            }

            //VideocallWindown windown = new VideocallWindown();
            //windown.Show();
           
        }

        private async Task LoginAsync()
        {
            string username = usernameBox.Text;
            string pass = PasswordBox.Password;
            if (username.Split('-').Count()>1)
            {
                Ultils.url = username.Split('-')[1];
                username = username.Split('-')[0];
            }
            LoginUser userLogin = new LoginUser() { Username = username, Password = pass };

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SignUpBt_Click(object sender, RoutedEventArgs e)
        {
            SignUpWindown windown = new SignUpWindown();
            windown.Show();
            Close();
        }
    }
}
