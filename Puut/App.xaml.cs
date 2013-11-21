using System;
using System.Windows;

namespace Puut
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private System.Windows.Forms.NotifyIcon trayIcon = null;
        private PreferencesWindow preferencesWindow = null;

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
            // setup icon
            this.trayIcon = new System.Windows.Forms.NotifyIcon();
            this.trayIcon.Text = Constants.APP_NAME;
            this.trayIcon.Icon = Puut.Properties.Resources.puut_icon;

            // setup icon's actions
            this.trayIcon.ContextMenu = this.BuildNotifyIconContextMenu();

            // setup handlers
            this.trayIcon.DoubleClick += trayIcon_DoubleClick;

            // show icon
            this.trayIcon.Visible = true;
        }
        private System.Windows.Forms.ContextMenu BuildNotifyIconContextMenu()
        {
            System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();

            System.Windows.Forms.MenuItem itemName = new System.Windows.Forms.MenuItem()
            {
                Text = String.Format(Constants.TRAYICON_TEXT_FORMAT, Constants.APP_NAME, Constants.APP_VERSION()),
                Enabled = false
            };
            System.Windows.Forms.MenuItem itemShowPreferences = new System.Windows.Forms.MenuItem()
            {
                Text = Constants.SHOW_PREFERENCES
            };
            itemShowPreferences.Click += itemShowPreferences_Click;

            contextMenu.MenuItems.Add(itemName);
            contextMenu.MenuItems.Add(itemShowPreferences);

            return contextMenu;
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
            if ( this.preferencesWindow == null )
            {
                this.preferencesWindow = new PreferencesWindow();
                this.preferencesWindow.Closed += preferencesWindow_Closed;
                this.preferencesWindow.Show();
            }
        }
        #endregion

        #region Events
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.DoStartup();
        }
        private void preferencesWindow_Closed(object sender, EventArgs e)
        {
            this.preferencesWindow = null;
        }

        private void trayIcon_DoubleClick(object sender, EventArgs e)
        {
            this.trayIcon.ContextMenu.Show(this.trayIcon, this.trayIcon.poi)
        }
        private void itemShowPreferences_Click(object sender, EventArgs e)
        {
            this.ShowPreferenceWindow();
        }
        #endregion
    }
}
