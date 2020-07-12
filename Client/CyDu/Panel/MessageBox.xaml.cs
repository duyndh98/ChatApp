using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using CyDu.Ultis;

namespace CyDu.Windown
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox : UserControl
    {
        public enum Side { User , Other  }
        public string Title { get; set; }
        public string Text { get; set; }
        public BitmapImage Avatar { get; set; }

        public MessageBox(string Title,string  Text,string arriveTime,long userId, Side side)
            {
            InitializeComponent();

            this.Text = Text;
            this.Title = Title;
            TileMess.Text = Title+"    "+arriveTime ;
            TextMess.Text = Text;
            Avatar = ImageSupportInstance.getInstance().GetUserImageFromId(userId);
            ImgMess1.Source = Avatar;
            ImgMess2.Source = Avatar;
            if (side==Side.User)
            {
                ImgMess1.Visibility = Visibility.Hidden;
                TextMess.Background = new SolidColorBrush(Color.FromArgb(100, 210, 222, 228));
                TextMess.HorizontalAlignment = HorizontalAlignment.Right;
                TileMess.HorizontalAlignment = HorizontalAlignment.Right;
                TileMess.Text = arriveTime;
            }
            else
            {
                ImgMess2.Visibility = Visibility.Hidden;
            }

        }
    }
}
