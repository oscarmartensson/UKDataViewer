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
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private SQLiteInteractor SQLiteDB;
        public MainWindow()
        {
            InitializeComponent();

            SQLiteDB = new SQLiteInteractor(this);
        }

        /// <summary>
        /// Displays the message in a pop-up window.
        /// </summary>
        /// <param name="message">Message to be displayed.</param>
        public void DisplayErrorMessage(string message)
        {
            MessageBox.Show(message, "Error occurred");
        }

        /// <summary>
        /// Triggers when the PropertyComboBox has changed item in the dropdown menu.
        /// </summary>
        /// <param name="sender">The instance that triggered the event.</param>
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
