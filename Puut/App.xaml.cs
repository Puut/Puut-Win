﻿using System;
using System.Drawing;
using System.IO;
using System.Windows;
using Puut.Capture;

namespace Puut
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application, IDisposable
    {
        private Window stubWindow = null;

        private System.Windows.Forms.NotifyIcon trayIcon = null;
        private PreferencesWindow preferencesWindow = null;

        #region Init
        private void DoStartup()
        {
            // Add event handlers
            this.Exit += App_Exit;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // to keep running
            this.ShowInvisibleWindow();

            this.AddNotifyIcon();

            // only show preferences window if nothing is set yet (server url and/or shortcut)
            if ( this.IsInsufficientStartup() )
            {
                this.ShowPreferenceWindow();
            }

            this.RegisterKeyHook();
        }
        private void App_Exit(object sender, ExitEventArgs e)
        {
            // we need to clean up!
            this.UnregisterKeyHook();
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
            System.Windows.Forms.MenuItem itemExit = new System.Windows.Forms.MenuItem()
            {
                Text = Constants.EXIT
            };
            itemExit.Click += itemExit_Click;

            contextMenu.MenuItems.Add(itemName);
            contextMenu.MenuItems.Add(itemShowPreferences);
            contextMenu.MenuItems.Add(itemExit);

            return contextMenu;
        }

        #region Keyboard Hotkey
        private void RegisterKeyHook()
        {
            HotKeyHelper.HotKeyPressed += HotKeyHelper_HotKeyPressed;

            const uint VK_F5 = 0x74;
            const uint MOD_CTRL = 0x0002;

            this.SetKeyHook(VK_F5, MOD_CTRL);
        }
        private void SetKeyHook(uint vkCode, uint modKeys)
        {
            HotKeyHelper.AddGlobalKeyHook(this.stubWindow, vkCode, modKeys);
        }
        private void UnregisterKeyHook()
        {
            HotKeyHelper.RemoveGlobalKeyHook(this.stubWindow);
        }
        #endregion
        #endregion

        #region Actions
        private void ShowInvisibleWindow()
        {
            this.stubWindow = new Window()
            {
                Width = 0,
                Height = 0,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                ShowActivated = false
            };
            this.stubWindow.Show();
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

        private void TakeScreenshot()
        {
            Console.WriteLine("Capturing the entire screen...");

            Image img = Screenshot.CaptureScreen();
            this.UploadScreenshot(img);
        }
        private void TakeRectangluarScreenshot()
        {
            Rectangle rect = new Rectangle();

            Console.WriteLine("Capturing rectangle " + rect + " ...");

            Image img = Screenshot.CaptureRectangle(rect);
            this.UploadScreenshot(img);
        }
        private async void UploadScreenshot(Image image)
        {
            Upload upload = new Upload();

            String username = null;
            String password = null;
            if ( Puut.Properties.Settings.Default.UsesAuth )
            {
                username = Puut.Properties.Settings.Default.Username;
                password = SecurityUtility.ToInsecureString(SecurityUtility.DecryptString(Puut.Properties.Settings.Default.Password));
            }

            Console.WriteLine("Uploading image...");
            String id = await upload.DoUpload(image, username, password);
            this.SetClipboardToId(id);
        }
        private void SetClipboardToId(String id)
        {
            if ( !String.IsNullOrEmpty(id) )
            {
                String host = Puut.Properties.Settings.Default.ServerURL;
                // to make Path.Combine use this
                if ( !host.EndsWith("/") )
                    host += "/";

                String url = Path.Combine(host, id);
                url += ".png";
                Console.WriteLine(url);

                // Clipboard.SetText(url); // crashing with CLIPBRD_E_CANT_OPEN
                Clipboard.SetDataObject(url);

                this.trayIcon.ShowBalloonTip(Constants.TOOLTIP_TIMEOUT, Constants.TOOLTIP_UPLOAD_TITLE, Constants.TOOLTIP_UPLOAD_BODY, Constants.TOOLTIP_UPLOAD_ICON);
            }
            else
            {
                this.trayIcon.ShowBalloonTip(Constants.TOOLTIP_TIMEOUT, Constants.TOOLTIP_UPLOADERROR_TITLE, Constants.TOOLTIP_UPLOADERROR_BODY, Constants.TOOLTIP_UPLOADERROR_ICON);
            }
        }
        #endregion

        #region Events
        // Application wide
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;

            MessageBox.Show(ex.ToString(), "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.DoStartup();
        }
        private void preferencesWindow_Closed(object sender, EventArgs e)
        {
            // invalidate object
            this.preferencesWindow = null;

            // TODO: update hotkey
        }
        private void HotKeyHelper_HotKeyPressed(object sender, EventArgs e)
        {
            Console.WriteLine("Hotkey pressed.");
            this.TakeScreenshot();
        }

        // Context menu of tray icon
        private void itemShowPreferences_Click(object sender, EventArgs e)
        {
            this.ShowPreferenceWindow();
        }
        private void itemExit_Click(object sender, EventArgs e)
        {
            this.Shutdown();
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            this.Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if ( disposing )
            {
                this.trayIcon.Dispose();
            }
        }
        ~App()
        {
            this.Dispose(false);
        }
        #endregion
    }
}
