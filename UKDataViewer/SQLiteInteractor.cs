using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Data.SQLite;

namespace UKDataViewer
{
    class SQLiteInteractor
    {
        private SQLiteConnection connection;

        // Loads the SQLite database from file which contains all personal
        // information necessary for the application to work.
        // Shuts down if the database couldn't be opened.
        public void LoadDatabase()
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

            GetMostCommonEmail();
        }

        private void GetMostCommonEmail()
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
                        emailAdresses[emailAdress]++;
                    }
                }
            }

            string mostCommonEmail = "";
            int max = 0;
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
        }
    }

}
