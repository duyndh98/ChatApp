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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CyDu.Model;
using CyDu.Ultis;

namespace CyDu.Panel
{
    /// <summary>
    /// Interaction logic for UserInfoPopup.xaml
    /// </summary>
    public partial class UserInfoPopup : UserControl
    {

        public event EventHandler LogoutEventhandler;
        public event EventHandler UpdateIconEventhandler;

        public UserInfoPopup()
        {
            InitializeComponent();
            lbuser.Text ="Username : "+ AppInstance.getInstance().GetUser().Username;
            lbfullname.Text = "Fullname : " + AppInstance.getInstance().GetUser().FullName;
            lbrole.Text = "Role : "+AppInstance.getInstance().GetUser().Role;
        }

        private void btlogout_Click(object sender, RoutedEventArgs e)
        {
            LogoutEventhandler(this, new EventArgs());
        }

        private void btUpdateAvatar_Click(object sender, RoutedEventArgs e)
        {

           string path = ImageSupportInstance.getInstance().OpenChooseImageDialogBox();
            if (!path.Equals(""))
            {
               Resource res = ImageSupportInstance.getInstance().UploadImage(path,360,360);
                User currentuser = AppInstance.getInstance().GetUser();
                currentuser.Avatar = res.Id;
                AppInstance.getInstance().SetUser(currentuser);

                UserSignup user = new UserSignup()
                {
                    Avatar = currentuser.Avatar,
                    FullName = currentuser.FullName,
                    Password = null,
                    Username = null
                };
                using (HttpClient client = new HttpClient())
                {
                    string url = Ultils.getUrl();
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                    HttpResponseMessage response = client.PutAsJsonAsync("/api/Users/Owner", user).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        UpdateIconEventhandler(this, new EventArgs());
                    }
                    else
                    {
                        _404Mess mess = response.Content.ReadAsAsync<_404Mess>().Result;
                        System.Windows.MessageBox.Show(mess.Message);
                    }
                }
            }
            
        }
    }
}
