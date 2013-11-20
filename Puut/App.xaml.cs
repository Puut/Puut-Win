using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Puut
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // only show preferences window if nothing is set yet (server url and/or shortcut)
            if ( String.IsNullOrEmpty(Puut.Properties.Settings.Default.ServerURL) || String.IsNullOrEmpty(Puut.Properties.Settings.Default.Shortcut) )
            {
                this.ShowPreferenceWindow();
            }
        }

        private void ShowPreferenceWindow()
        {
            new PreferencesWindow().Show();
        }
    }
}
