using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace CyDu.Windown
{
    /// <summary>
    /// Interaction logic for ImageWindown.xaml
    /// </summary>
    public partial class ImageWindown : Window
    {
        public ImageWindown(string imgBase64img)
        {
            InitializeComponent();
            byte[] data = System.Convert.FromBase64String(imgBase64img);
            MemoryStream memoryStream = new MemoryStream(data);
            var imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.StreamSource = memoryStream;
            imageSource.EndInit();
            imgMain.Source = imageSource;
        }
    }
}
