﻿using System;
using System.Windows;

namespace Puut
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private System.Windows.Forms.NotifyIcon trayIcon = null;

        #region Init
        private void DoStartup()
        {
            // to keep running
            this.ShowInvisibleWindow();

            this.AddNotifyIcon();

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
            this.trayIcon = new System.Windows.Forms.NotifyIcon();
            this.trayIcon.Text = String.Format(Constants.TRAYICON_TEXT_FORMAT, Constants.APP_NAME, Constants.APP_VERSION());
            this.trayIcon.Icon = Puut.Properties.Resources.puut_icon;

            this.trayIcon.Visible = true;
        }
        #endregion

        #region Actions
        private void ShowInvisibleWindow()
        {
            new Window()
            {
                Width = 0,
                Height = 0,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                ShowActivated = false
            }.Show();
        }
        private void ShowPreferenceWindow()
        {
            new PreferencesWindow().Show();
        }
        #endregion

        #region Events
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.DoStartup();
        }
        #endregion
    }
}
