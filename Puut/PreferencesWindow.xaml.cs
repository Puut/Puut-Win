using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Puut.Properties;

namespace Puut
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class PreferencesWindow : MetroWindow
    {
        #region Init
        public PreferencesWindow()
        {
            InitializeComponent();

            this.LoadSettings();
            this.UpdateUserDataFields();
        }
        #endregion

        #region Load/Save Settings
        private void LoadSettings()
        {
            Settings s = Puut.Properties.Settings.Default;

            this.textBoxUrl.Text = s.ServerURL;
            this.checkBoxUsesAuth.IsChecked = s.UsesAuth;
            this.textBoxUsername.Text = s.Username;
            this.textBoxPassword.Password = SecurityUtility.ToInsecureString(SecurityUtility.DecryptString(s.Password));
            this.textBoxShortcut.Text = s.Shortcut;
        }

        private void SaveSettings()
        {
            Settings s = Puut.Properties.Settings.Default;

            s.ServerURL = this.textBoxUrl.Text;
            s.UsesAuth = this.checkBoxUsesAuth.IsChecked.Value;
            s.Username = this.textBoxUsername.Text;
            s.Password = SecurityUtility.EncryptString(this.textBoxPassword.SecurePassword);
            s.Shortcut = this.textBoxShortcut.Text;
            s.Save();
        }
        #endregion

        #region Event handlers for UI
        private void shortcutTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            this.HandleShortcutBox(e);
        }

        private void textboxes_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.UpdateUserDataFields();
        }
        private void usesAuthCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateUserDataFields();
        }
        private void usesAuthCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateUserDataFields();
        }
        private void passwordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.UpdateUserDataFields();
        }
        
        /// <summary>
        /// Event handler for when the "apply" button had been pressed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event args.</param>
        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            // save settings
            SaveSettings();
            // update ui
            this.UpdateUserDataFields();
        }
        #endregion

        #region UI
        private void UpdateUserDataFields()
        {
            Settings s = Puut.Properties.Settings.Default;
            bool settingsChanged = false;

            this.textBoxUsername.IsEnabled = this.textBoxPassword.IsEnabled = ( this.checkBoxUsesAuth.IsChecked.Value );

            settingsChanged |= ( this.textBoxShortcut.Text != s.Shortcut );
            settingsChanged |= ( this.textBoxUrl.Text != s.ServerURL );
            settingsChanged |= ( this.checkBoxUsesAuth.IsChecked.Value != s.UsesAuth );
            settingsChanged |= ( this.textBoxUsername.Text != s.Username );
            settingsChanged |= ( this.textBoxPassword.Password != SecurityUtility.ToInsecureString(SecurityUtility.DecryptString(s.Password)) );

            this.buttonApply.IsEnabled = settingsChanged;
        }

        private void HandleShortcutBox(KeyEventArgs e)
        {
            // The text box grabs all input.
            e.Handled = true;

            // Fetch the actual shortcut key.
            Key key = ( e.Key == Key.System ? e.SystemKey : e.Key );

            // Ignore modifier keys.
            if ( key == Key.LeftShift || key == Key.RightShift
                || key == Key.LeftCtrl || key == Key.RightCtrl
                || key == Key.LeftAlt || key == Key.RightAlt
                || key == Key.LWin || key == Key.RWin )
            {
                return;
            }

            // Build the shortcut key name.
            StringBuilder shortcutText = new StringBuilder();
            if ( ( Keyboard.Modifiers & ModifierKeys.Control ) != 0 )
            {
                shortcutText.Append("Ctrl+");
            }
            if ( ( Keyboard.Modifiers & ModifierKeys.Shift ) != 0 )
            {
                shortcutText.Append("Shift+");
            }
            if ( ( Keyboard.Modifiers & ModifierKeys.Alt ) != 0 )
            {
                shortcutText.Append("Alt+");
            }
            shortcutText.Append(key.ToString());

            // Update the text box.
            this.textBoxShortcut.Text = shortcutText.ToString();

            this.UpdateUserDataFields();
        }
        #endregion
    }
}
