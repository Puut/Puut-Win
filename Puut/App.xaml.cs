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
        #region Init
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // only show preferences window if nothing is set yet (server url and/or shortcut)
            if ( this.IsInsufficientStartup() )
            {
                this.ShowPreferenceWindow();
            }
        }
        /// <summary>
        /// Returns whether all basic information was set, so that we could try to puut files.
        /// </summary>
        /// <returns>True if the server and the shortcut were set. False otherwise.</returns>
        private bool IsInsufficientStartup()
        {
            return ( String.IsNullOrEmpty(Puut.Properties.Settings.Default.ServerURL) || String.IsNullOrEmpty(Puut.Properties.Settings.Default.Shortcut) );
        }

        private void AddNotifyIcon()
        {

        }
        #endregion

        #region Actions
        private void ShowPreferenceWindow()
        {
            new PreferencesWindow().Show();
        }
        #endregion
    }
}
