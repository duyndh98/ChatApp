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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CyDu.Windown
{
    /// <summary>
    /// Interaction logic for ContactListControl.xaml
    /// </summary>
    /// 


    public partial class ContactListControl : UserControl
    {
        public ObservableCollection<ContactListItem> Constacts { get; set; }

        public ContactListControl(ObservableCollection<ContactListItem> Constacts)
        {
            InitializeComponent();

            lvContact.ItemsSource = Constacts;

            CollectionView view =  (CollectionView)CollectionViewSource.GetDefaultView(lvContact.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("FirsChar");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void Btadd_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
