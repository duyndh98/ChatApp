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
using CyDu.Model;
using CyDu.Ultis;

namespace CyDu.Dialogs
{
    /// <summary>
    /// Interaction logic for EditConversationNameDialog.xaml
    /// </summary>
    public partial class EditConversationNameDialog : Window
    {
        private string coversationNamelast;
        private long conversaionId;
        public EditConversationNameDialog(string conversionname,long conversationid)
        {
            InitializeComponent();
            coversationNamelast = conversionname;
            this.conversaionId = conversationid;
            titletb.Text = "Change conversation name from " + conversionname + " to :";
        }

        private void tbcancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void tbok_Click(object sender, RoutedEventArgs e)
        {
            string text = conversaionnametb.Text.Trim();
            if (!text.Equals(""))
            {
                Conversation conver = new Conversation() { Name = text };
            using (HttpClient client = new HttpClient())
                {
                    string url = Ultils.getUrl();
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                    HttpResponseMessage response = client.PutAsJsonAsync("/api/Conversations/"+this.conversaionId, conver).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        System.Windows.MessageBox.Show("Lỗi khi gửi tin");
                    }
                }
                this.DialogResult = true;
            }
            else
            {
                conversaionnametb.Text = coversationNamelast;
            }
           
        }
    }
}
