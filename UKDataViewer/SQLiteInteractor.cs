using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

using UKDataViewer.Exceptions;

namespace UKDataViewer
{

    /// <summary>
    /// A class that acts as an interactor with the SQLite
    /// database holding all relevant information for this
    /// application.
    /// </summary>
    class SQLiteInteractor
    {
        /// <summary>
        /// Whether this instance was initialized or not.
        /// </summary>
        private bool isInitialized = false;

        /// <summary>
        /// Instance of client that makes REST queries to endpoint url.
        /// </summary>
        private PostcodesClient restClient;

        /// <summary>
        /// Reference to main window.
        /// </summary>
        private MainWindow mainWindow;

        /// <summary>
        /// Path to the database file.
        /// </summary>
        private string databasePath;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="window">Reference to the main window.</param>
        public SQLiteInteractor(MainWindow window)
        {
            mainWindow = window;

            string currentDir = Directory.GetCurrentDirectory();
            string dbPath = currentDir + @"\uk-500.db";

            if (!File.Exists(dbPath))
            {
                mainWindow.DisplayErrorMessage("Database 'uk-data.db' doesn't exist. Exiting program.");
                Environment.Exit(1);
            }

            databasePath = dbPath;

            restClient = new PostcodesClient();

            isInitialized = true;
        }

        /// <summary>
        /// Whether the SQLiteInteractor is initialized or not.
        /// </summary>
        /// <returns>Current initialization status.</returns>
        public bool IsInitialized()
        {
            return isInitialized;
        }

        /// <summary>
        /// Fetches emails from the database and finds the most
        /// common one.
        /// </summary>
        /// <returns>Name of the most common email.</returns>
        public string GetMostCommonEmail()
        {
            var connection = new SQLiteConnection(@"URI=file:" + databasePath);
            connection.Open();

            // Get the email addresses from the database by querying the connection.
            string query = "SELECT email FROM ukdata";
            SQLiteCommand command = new SQLiteCommand(query, connection);
            SQLiteDataReader reader = command.ExecuteReader();

            Dictionary<string,int> emailAdresses = new Dictionary<string,int>();

            while (reader.Read())
            {
                // Read the query. We know that the email is a string, and there is
                // only one column, so no worry with index out of bounds.
                string email = reader.GetString(0);
                int emailIndex = email.IndexOf("@");
                if (emailIndex != -1)
                {
                    // No need to store the '@' sign, so increment by 1.
                    string emailAdress = email.Substring(emailIndex + 1);
                    if (!emailAdresses.ContainsKey(emailAdress))
                    {
                        emailAdresses.Add(emailAdress, 0);
                    } else
                    {
                        // Address already exists, increment counter for
                        // number of times this email address has been found.
                        emailAdresses[emailAdress]++;
                    }
                }
            }

            string mostCommonEmail = "";
            int max = 0;
            // Go through all the different email addresses found and extract
            // the most common one (i.e. highest Value in emailAdresses.)
            foreach (var email in emailAdresses)
            {
                int val = email.Value;
                if (val > max)
                {
                    mostCommonEmail = email.Key;
                    max = val;
                }
            }

            connection.Close();

            return mostCommonEmail;
        }

        /// <summary>
        /// Queries the database for postcodes, then make REST API
        /// calls using the PostcodeClient class to get the longitude
        /// and latitude position for the postcodes. Using this data,
        /// clusters are computed, grouping the postcodes together
        /// spatially.
        /// </summary>
        public async Task<List<DBSCAN.Cluster<ClusterInfo>>> GetClusterData(double searchRadius = 10000.0, int clusterSize = 3)
        {
            var connection = new SQLiteConnection(@"URI=file:" + databasePath);
            connection.Open();

            // Get the postcodes from the database by querying the connection.
            string query = "SELECT postal FROM ukdata";
            SQLiteCommand command = new SQLiteCommand(query, connection);
            SQLiteDataReader reader = command.ExecuteReader();

            List<string> postcodes = new List<string>();

            while (reader.Read())
            {
                // Read the answer to the database query.
                postcodes.Add(reader.GetString(0));
            }

            // Get all useful data for the cluster display.
            query = "SELECT first_name, county, city FROM ukdata";
            command = new SQLiteCommand(query, connection);
            reader = command.ExecuteReader();

            List<ClusterInfo> clusterData = new List<ClusterInfo>();
            while (reader.Read())
            {
                // Read the answer to the database query.
                ClusterInfo clusterInfo =
                    new ClusterInfo(reader.GetString(0),
                                    reader.GetString(1),
                                    reader.GetString(2));
                clusterData.Add(clusterInfo);
            }


            connection.Close();

            // Send bulk queries with the postcodes to the restClient to get longitude and latitude positional data
            // in order to calculate spatial clusters. 100 postcodes are sent at a time (max allowed by site)
            // to prevent sending too many single requests.
            int queryCount = 100;

            // We're only interested in the longitude and latitude for cluster computation.
            string postcodesQuery = "postcodes?filter=longitude,latitude";
            List<BulkQueryResult<string, LongLat>> longLats = new List<BulkQueryResult<string, LongLat>>();
            for (int i = 0; i < postcodes.Count; i += queryCount)
            {
                if (i + queryCount < postcodes.Count)
                {
                    try
                    {
                        var queryResult = restClient.BulkPostcodeLookup<LongLat>(postcodes.GetRange(i, queryCount), postcodesQuery);

                        if (queryResult != null)
                        {
                            longLats.AddRange(await queryResult);
                        }
                    }
                    catch (RESTException /*e*/)
                    {
                        mainWindow.DisplayErrorMessage("Error connecting to Postcodes.IO, check your Internet connection. Cluster data won't be able to be shown.");
                        return null;
                    }
                    catch (BadStatusException e)
                    {
                        mainWindow.DisplayErrorMessage(e.GetBaseException().Message);
                        return null;
                    }
                }
                else
                {
                    // There's < 100 elements left in array until we 
                    // hit the end. Get number of elements left and get that range.
                    int remainder = postcodes.Count - i - 1;

                    try
                    {
                        var queryResult = restClient.BulkPostcodeLookup<LongLat>(postcodes.GetRange(i, remainder), postcodesQuery);

                        if (queryResult != null)
                        {
                            longLats.AddRange(await queryResult);
                        }
                    }
                    catch (RESTException /*e*/)
                    {
                        mainWindow.DisplayErrorMessage("Error connecting to Postcodes.IO, check your Internet connection. Cluster data won't be able to be shown.");
                        return null;
                    }
                    catch (BadStatusException e)
                    {
                        mainWindow.DisplayErrorMessage(e.GetBaseException().Message);
                        return null;
                    }
                }
            }

            List<DBSCAN.PointInfo<ClusterInfo>> coords = new List<DBSCAN.PointInfo<ClusterInfo>>(longLats.Count);
            for (int i = 0; i < longLats.Count; i++)
            {
                if (longLats[i].Result != null)
                {
                    coords.Add(new DBSCAN.PointInfo<ClusterInfo>(new ClusterInfo(clusterData[i].name,
                                                                                 clusterData[i].county,
                                                                                 clusterData[i].city,
                                                                                 longLats[i].Result.Longitude,
                                                                                 longLats[i].Result.Latitude)));
                }
            }

            // Create a data structure that fits the interface of the DBSCAN algorithm when using a custom distance function.
            DBSCAN.ListSpatialIndex<DBSCAN.PointInfo<ClusterInfo>> locations = new DBSCAN.ListSpatialIndex<DBSCAN.PointInfo<ClusterInfo>>(coords, ClusterInfo.DistanceFunction);

            // Run the DBSCAN cluster algorithm to group the data together.
            var clusters = DBSCAN.DBSCAN.CalculateClusters(index: locations, searchRadius, clusterSize);

            if (clusters == null)
            {
                // algorithm didn't provide a reasonable answer or something went wrong.
                return null;
            }

            // Sort clusters so that the largest one is in the first index of the list, etc.
            var sortedClusters = clusters.Clusters.OrderBy(x => x.Objects.Count).Reverse().ToList();

            return sortedClusters;
        }
    }
}
