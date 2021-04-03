
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.Linq;

namespace UKDataViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Max number of clusters to be displayed.
        /// </summary>
        private readonly int maxClusters = 3;

        /// <summary>
        /// Cluster data.
        /// </summary>
        private List<DBSCAN.Cluster<ClusterInfo>> clusters = null;

        /// <summary>
        /// Interacts with database and gets data to be displayed in GUI.
        /// </summary>
        private SQLiteInteractor sqliteDB;

        /// <summary>
        /// Cluster names and cluster position to check against
        /// out of bounds for number of clusters calculated.
        /// </summary>
        private Dictionary<string, int> clusterNames;

        private ObservableCollection<ClusterInfo> clusterCollection = new ObservableCollection<ClusterInfo>();
        public MainWindow()
        {
            InitializeComponent();

            sqliteDB = new SQLiteInteractor(this);

            // Set default values for application GUI.
            this.SearchRadius.Text = "10000";
            this.ClusterSizeInput.Text = string.Format("{0}", maxClusters);

            // Add maximum of 3 possible clusters to display.
            this.clusterNames = new Dictionary<string, int>(maxClusters);
            this.clusterNames.Add("Cluster 1", 0);
            this.clusterNames.Add("Cluster 2", 1);
            this.clusterNames.Add("Cluster 3", 2);

            this.ClusterView.ItemsSource = clusterCollection;
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
        /// <param name="sender">The item that triggered the event.</param>
        /// <param name="e">Information about the event.</param>
        private void PropertyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sqliteDB != null && sqliteDB.IsInitialized())
            {
                ComboBox cb = (ComboBox)sender;
                ComboBoxItem item = (ComboBoxItem)cb.SelectedItem;

                switch((string)item.Content)
                {
                    case "Email":
                    default:
                        this.PropertyTextBox.Text = sqliteDB.GetMostCommonEmail();
                        break;
                }
            }
        }

        /// <summary>
        /// Action logic for when cluster button is pushed.
        /// </summary>
        /// <param name="sender">The item that triggered the event.</param>
        /// <param name="e">Information about the event.</param>
        private async void ApplyClusterParams_Click(object sender, RoutedEventArgs e)
        {
            // Must be positive values without decimals.
            var styles = NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite;
            var provider = NumberFormatInfo.InvariantInfo;

            bool isDoubleSearchRadius = double.TryParse(this.SearchRadius.Text, styles, provider, out double searchRadius);
            bool isIntClusterSize = int.TryParse(this.ClusterSizeInput.Text, styles, provider, out int clusterSize);

            if (isDoubleSearchRadius && isIntClusterSize && sqliteDB != null)
            {
                // Both input parameters are valid.
                this.clusters = await sqliteDB.GetClusterData(searchRadius, clusterSize);
                this.ClusterComboBox.Items.Clear();

                if (clusters != null)
                {
                    int nrOfClusters = clusters.Count > 3 ? maxClusters : clusters.Count;
                    for (int i = 0; i < nrOfClusters; i++)
                    {
                        // Since clusterNames is small, ElementAt works fine without performance hit.
                        this.ClusterComboBox.Items.Add(clusterNames.Keys.ElementAt(i));
                    }
                    if (this.ClusterComboBox.Items.Count > 0)
                    {
                        this.ClusterComboBox.SelectedItem = this.ClusterComboBox.Items.GetItemAt(0);
                    }
                }
            }
            else
            {
                DisplayErrorMessage("Invalid cluster parameter input. Must be whole positive digits without, punctuation, comma or sign.");
            }
        }

        /// <summary>
        /// Triggers when the ClusterComboBox has changed item in the dropdown menu.
        /// </summary>
        /// <param name="sender">The item that triggered the event.</param>
        /// <param name="e">Information about the event.</param>
        private void ClusterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (clusterCollection.Count > 0)
            {
                // If we have some values in it already, clear it.
                clusterCollection.Clear();
                this.ClusterSizeOuput.Text = "";
            }

            ComboBox cb = (ComboBox)sender;

            if (cb.SelectedItem == null || !clusterNames.TryGetValue((string)cb.SelectedItem, out int clusterIndex) || clusters == null )
            {
                return;
            }

            int nrOfClusters = clusters.Count;
            if (clusterIndex > nrOfClusters - 1)
            {
                // Selected cluster will be outside cluster list.
                return;
            }

            int clusterSize = clusters[clusterIndex].Objects.Count;
            this.ClusterSizeOuput.Text = string.Format("{0}", clusterSize);
            for (int j = 0; j < clusterSize - 1; j++)
            {
                // Update all data shown in the ListView.
                clusterCollection.Add(clusters[clusterIndex].Objects[j]);
            }
        }
    }
}
