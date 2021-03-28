using System;
using System.Collections.Generic;
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

namespace UKDataViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SQLiteInteractor SQLiteDB;
        public MainWindow()
        {
            InitializeComponent();

            // Create instance of the SQLiteInteractor and load in the database.
            SQLiteDB = new SQLiteInteractor();
            SQLiteDB.Initialize();
        }

        private void PropertyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SQLiteDB != null && SQLiteDB.IsInitialized())
            {
                ComboBox cb = (ComboBox)sender;
                ComboBoxItem item = (ComboBoxItem)cb.SelectedItem;

                string propertyString = "";
                switch((string)item.Content)
                {
                    case "Email":
                    default:
                        propertyString = SQLiteDB.GetMostCommonEmail();
                        break;
                }

                this.PropertyTextBox.Text = propertyString;
            }
        }
    }
}
