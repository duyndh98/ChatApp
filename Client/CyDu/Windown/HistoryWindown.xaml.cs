using CyDu.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for HistoryWindown.xaml
    /// </summary>
    public partial class HistoryWindown : UserControl
    {
        public ObservableCollection<ConversationsView> History { get; set; }
        public event EventHandler HistoryEventHandler;

        public HistoryWindown(ObservableCollection<ConversationsView> _listhistory)
        {
            InitializeComponent();
            History = _listhistory;
            lvHistory.ItemsSource = _listhistory;
        }

     
        public void Refresh()
        {

        }

        private void Btadd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lvHistory_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListView lv = sender as ListView;
            HistoryEventHandler(this, new HistoryItemSelectedArgs() {
                itemIndex = lv.SelectedIndex ,
                pk_seq = History.ElementAt(lv.SelectedIndex).Pk_seq,
                username = History.ElementAt(lv.SelectedIndex).Username
            });
          
        }

        private void FrameworkElement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }

    public class HistoryItemSelectedArgs : EventArgs
    {
        public int itemIndex { get; set; }
        public long pk_seq { get; set; }
        public string username { get; set; }
    }
}
