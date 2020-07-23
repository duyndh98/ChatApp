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
using CyDu.Dialogs;
using CyDu.Model;
using CyDu.Ultis;

namespace CyDu.Windown
{
    /// <summary>
    /// Interaction logic for SignUpWindown.xaml
    /// </summary>
    public partial class SignUpWindown : Window
    {
        private string avatapath;
        public SignUpWindown()
        {
            InitializeComponent();
            avatapath = @"E:\WorkspaceVS19\Git\ChatApp\Client\CyDu\img\userico.png";
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            string password = PasswordBox.Password;
            string repassword = RePasswordBox.Password;
            if (password.Equals(repassword))
            {
                string user = usernameBox.Text.Trim();
                if (!user.Equals(""))
                {
                    //chưa authorized
                    string fullname = fullnameBox.Text;
                    UserSignup signup = new UserSignup()
                    {
                        FullName = fullname,
                        Password = password,
                        Username = user,
                        Avatar = 0
                    };
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(Ultis.Ultils.url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = client.PostAsJsonAsync("/api/Users/Register", signup).Result;
                        if (response.IsSuccessStatusCode)
                        {
                      
                            LoginWindown windown = new LoginWindown();
                            windown.SetAccount(user, password);
                            windown.Show();
                            this.Close();
                        }
                        else
                        {
                            _404Mess mess = response.Content.ReadAsAsync<_404Mess>().Result;
                            ErrLable.Content = mess.Message;
                        }
                    }
                }
                else
                {
                    ErrLable.Content = "Username not empty";
                }
            }
            else
            {
                ErrLable.Content = "Password is not match";
            }
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void loginbt_Click(object sender, RoutedEventArgs e)
        {
            LoginWindown lgwindown = new LoginWindown();
            lgwindown.Show();
        }
    }
}
