using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
            SQLiteDB = new SQLiteInteractor(this);
        }

        public void DisplayErrorMessage(string message)
        {
            MessageBox.Show(message, "Error occurred");
        }

        private void PropertyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SQLiteDB != null && SQLiteDB.IsInitialized())
            {
                ComboBox cb = (ComboBox)sender;
                ComboBoxItem item = (ComboBoxItem)cb.SelectedItem;

                switch((string)item.Content)
                {
                    case "Email":
                    default:
                        this.PropertyTextBox.Text = SQLiteDB.GetMostCommonEmail();
                        break;
                }

            }
        }
    }
}
