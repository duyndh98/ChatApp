using CyDu.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for MainChatWindown.xaml
    /// </summary>
    public partial class MainChatWindown : Window
    {
        public ObservableCollection<HistoryListItem> items;
        public MainChatWindown()
        {
           
            InitializeComponent();
            items = new ObservableCollection<HistoryListItem>()
            {
                new HistoryListItem
                {
                    Username = "cy",
                    Text = "alo alo"
                },
                new HistoryListItem
                {
                    Username = "Trường",
                    Text = "1 2 3"
                },
                new HistoryListItem
                {
                    Username = "cy",
                    Text = "alo alo"
                },
                new HistoryListItem
                {
                    Username = "Trường",
                    Text = "1 2 3"
                },new HistoryListItem
                {
                    Username = "cy",
                    Text = "alo alo"
                },
                new HistoryListItem
                {
                    Username = "Trường",
                    Text = "1 2 3"
                },new HistoryListItem
                {
                    Username = "cy",
                    Text = "alo alo"
                },
                new HistoryListItem
                {
                    Username = "Trường",
                    Text = "1 2 3"
                },new HistoryListItem
                {
                    Username = "cy",
                    Text = "alo alo"
                },
                new HistoryListItem
                {
                    Username = "Trường",
                    Text = "1 2 3"
                }
            };
            HistoryWindown Historywindown = new HistoryWindown(items);
            Historywindown.Name = "Historywd";
            Historywindown.HistoryEventHandler += Windown_HistoryEventHandler;
            //ObservableCollection<ContactListItem> itemss = new ObservableCollection<ContactListItem>()
            //{
            //    new ContactListItem()
            //    {
            //        Username = "Cy",
            //        FirsChar ='C'
            //    },
            //    new ContactListItem()
            //    {
            //        Username = "CYlasion",
            //        FirsChar ='C'
            //    },
            //    new ContactListItem()
            //    {
            //        Username = "Aaaa",
            //        FirsChar ='a'

            //    }
            //};
            //ContactListControl control = new ContactListControl(itemss);
            chattingPanel.ChatPannelTitle = "Phạm Nhật Trường";
            LeftPanel.Children.Add(Historywindown);
        }

        private void Windown_HistoryEventHandler(object sender, EventArgs e)
        {
            HistoryItemSelectedArgs itemindex = e as HistoryItemSelectedArgs;
        }

        private void setChatpanel(object sender , HistoryItemSelectedArgs e )
        {

        }
    }
}
