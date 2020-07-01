using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using CyDu.Ultis;
using System.Net.Http.Headers;
using CyDu.Model;

namespace CyDu.Dialogs
{
    /// <summary>
    /// Interaction logic for AddNewContactDialog.xaml
    /// </summary>
    public partial class AddNewContactDialog : Window
    {
        public AddNewContactDialog()
        {
            InitializeComponent();
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            string username = tbInput.Text;
            long ToUserId = 0;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Ultils.url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage respone = client.GetAsync("/api/Users/ByUserName/"+ username, HttpCompletionOption.ResponseContentRead).Result;
                if (respone.IsSuccessStatusCode)
                {
                    User user = respone.Content.ReadAsAsync<User>().Result;
                    ToUserId = user.Id;
                }
                else
                {
                    _404Mess mess = respone.Content.ReadAsAsync<_404Mess>().Result;
                    errlb.Text = mess.Message;
                }
            }

            if (ToUserId!=0 && AppInstance.getInstance().GetUser().Id != ToUserId)
            {
                Contact contact = new Contact() { ToUserId = ToUserId };
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Ultils.url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                    HttpResponseMessage respone = client.PostAsJsonAsync("/api/Contacts/", contact).Result;
                    if (respone.IsSuccessStatusCode)
                    {
                        this.DialogResult = true;
                    }
                    else
                    {
                        _404Mess mess = respone.Content.ReadAsAsync<_404Mess>().Result;
                        errlb.Text = mess.Message;
                    }
                }
            }
           
        }

        private void btcancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }
    }
}
