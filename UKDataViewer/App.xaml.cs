using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace UKDataViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create instance of the SQLiteInteractor and load in the database.
            SQLiteInteractor SQLiteDB = new SQLiteInteractor();
            SQLiteDB.LoadDatabase();
        }
    }
}
