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

namespace CyDu.Windown
{
    /// <summary>
    /// Interaction logic for ChattingPanel.xaml
    /// </summary>
    public partial class ChattingPanel : UserControl
    {
        public string ChatPannelTitle { get; set; }

        public ChattingPanel()
        {

            InitializeComponent();
            this.DataContext = this;
            List<MessageBox> boxs = new List<MessageBox>()
            {
                new MessageBox( "Cy - 1ty3 năm trc", "How to summon a legal loli",MessageBox.Side.User)
          ,
                new MessageBox( "Trường- 11h trước ","An exception is a situation, which occured by the runtime error. In other words, an exception is a runtime error. An exception may result in loss of data or an abnormal execution of program.",MessageBox.Side.Other)

        ,
                new MessageBox( "Cy - 1ty3 năm trc", "How to summon a legal loli",MessageBox.Side.User)
          ,
                new MessageBox( "Trường- 11h trước ","An exception is a situation, which occured by the runtime error. In other words, an exception is a runtime error. An exception may result in loss of data or an abnormal execution of program.",MessageBox.Side.Other)

,
                new MessageBox( "Cy - 1ty3 năm trc", "How to summon a legal loli",MessageBox.Side.User)
          ,
                new MessageBox( "Trường- 11h trước ","An exception is a situation, which occured by the runtime error. In other words, an exception is a runtime error. An exception may result in loss of data or an abnormal execution of program.",MessageBox.Side.Other)

,
                new MessageBox( "Cy - 1ty3 năm trc", "How to summon a legal loli",MessageBox.Side.User)
          ,
                new MessageBox( "Trường- 11h trước ","An exception is a situation, which occured by the runtime error. In other words, an exception is a runtime error. An exception may result in loss of data or an abnormal execution of program.",MessageBox.Side.Other)
   };

            foreach (MessageBox box in boxs)
            {
                mainPanel.Children.Add(box);
            }

            mainScroll.ScrollToBottom();
        }
    }
}
