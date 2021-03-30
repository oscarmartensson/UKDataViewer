using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;

using UKDataViewer.Exceptions;
using DBSCAN;

namespace UKDataViewer
{
    class SQLiteInteractor
    {
        private SQLiteConnection connection;
        private bool isInitialized = false;

        private PostcodesClient restClient;
        private MainWindow mainWindow;

        public SQLiteInteractor(MainWindow _window)
        {
            LoadDatabase();

            restClient = new PostcodesClient();

            mainWindow = _window;

            isInitialized = true;

            GetPoscodeLocations();
        }

        // Loads the SQLite database from file which contains all personal
        // information necessary for the application to work.
        // Shuts down if the database couldn't be opened.
        private void LoadDatabase()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string dbPath = currentDir + @"\uk-500.db";
            Console.WriteLine(dbPath);

            if (!File.Exists(dbPath))
            {
                Console.WriteLine("Database 'uk-data.db' doesn't exist. Exiting program.");
                Environment.Exit(1);
            }

            try
            {
                // Try to open the database to make sure it works.
                connection = new SQLiteConnection(@"URI=file:" + dbPath);
                connection.Open();
            }
            catch
            {
                Console.WriteLine("Database 'uk-data.db' couldn't be opened. Exiting program.");
                Environment.Exit(1);
            }

            connection.Close();
        }

        public bool IsInitialized()
        {
            return isInitialized;
        }

        public string GetMostCommonEmail()
        {
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

        public void GetPoscodeLocations()
        {
            connection.Open();

            // Get the poscodes from the database by querying the connection.
            string query = "SELECT postal FROM ukdata";
            SQLiteCommand command = new SQLiteCommand(query, connection);
            SQLiteDataReader reader = command.ExecuteReader();

            List<string> postcodes = new List<string>();
            while (reader.Read())
            {
                postcodes.Add(reader.GetString(0));
            }

            connection.Close();

            int queryCount = 100;
            List<BulkQueryResult<string, LongLat>> longLats = new List<BulkQueryResult<string, LongLat>>();
            for (int i = 0; i < postcodes.Count; i += queryCount)
            {
                if (i + queryCount < postcodes.Count)
                {
                    try
                    {
                        var queryResult = restClient.BulkPostcodeLookup(postcodes.GetRange(i, queryCount));

                        if (queryResult != null)
                        {
                            longLats.AddRange(queryResult);
                        }
                    }
                    catch (RESTException e)
                    {
                        mainWindow.DisplayErrorMessage("Error connecting to Postcodes.IO, check your internet connection. Cluster data won't be able to be shown.");
                        return;
                    }
                    catch (BadStatusException e)
                    {
                        mainWindow.DisplayErrorMessage(e.GetBaseException().Message);
                        return;
                    }
                }
                else
                {
                    // There's < 100 elements left in array until we 
                    // hit the end. Get number of elements left and get that range.
                    int remainder = postcodes.Count - i;

                    try
                    {
                        var queryResult = restClient.BulkPostcodeLookup(postcodes.GetRange(i, remainder));

                        if (queryResult != null)
                        {
                            longLats.AddRange(queryResult);
                        }
                    }
                    catch (RESTException /*e*/)
                    {
                        mainWindow.DisplayErrorMessage("Error connecting to Postcodes.IO, check your internet connection. Cluster data won't be able to be shown.");
                        return;
                    }
                    catch (BadStatusException e)
                    {
                        mainWindow.DisplayErrorMessage(e.GetBaseException().Message);
                        return;
                    }
                }
            }

            List<PointInfo<Location>> coords = new List<PointInfo<Location>>(longLats.Count);
            for (int i = 0; i < longLats.Count; i++)
            {
                if(longLats[i].Result != null)
                {
                    coords.Add(new PointInfo<Location>(new Location(longLats[i].Result.Longitude, longLats[i].Result.Latitude)));
                }
            }

            ListSpatialIndex<PointInfo<Location>> locations = new ListSpatialIndex<PointInfo<Location>>(coords, Location.DistanceFunction);
            var cluster = DBSCAN.DBSCAN.CalculateClusters(index: locations, 10000.0, 3);
        }
    }
}
