using System.Text;
using System.Windows;
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

        #region Load/Save
        private void LoadSettings()
        {
            Settings s = Puut.Properties.Settings.Default;

            urlTextBox.Text = s.ServerURL;
            usesAuthCheckbox.IsChecked = s.UsesAuth;
            usernameTextBox.Text = s.Username;
            passwordTextBox.Password = SecurityUtility.ToInsecureString(SecurityUtility.DecryptString(s.Password));
            shortcutTextBox.Text = s.Shortcut;
        }

        private void SaveSettings()
        {
            Settings s = Puut.Properties.Settings.Default;

            s.ServerURL = urlTextBox.Text;
            s.UsesAuth = usesAuthCheckbox.IsChecked.Value;
            s.Username = usernameTextBox.Text;
            s.Password = SecurityUtility.EncryptString(passwordTextBox.SecurePassword);
            s.Shortcut = shortcutTextBox.Text;
            s.Save();
        }
        #endregion

        #region Event handlers for UI changes
        private void shortcutTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            this.HandleShortcutBox(e);
        }

        private void urlTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
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
        private void usernameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.UpdateUserDataFields();
        }
        private void passwordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.UpdateUserDataFields();
        }
        #endregion

        #region UI
        private void UpdateUserDataFields()
        {
            Settings s = Puut.Properties.Settings.Default;
            bool settingsChanged = false;

            usernameTextBox.IsEnabled = passwordTextBox.IsEnabled = ( usesAuthCheckbox.IsChecked.Value );

            settingsChanged |= ( shortcutTextBox.Text != s.Shortcut );
            settingsChanged |= ( urlTextBox.Text != s.ServerURL );
            settingsChanged |= ( usesAuthCheckbox.IsChecked.Value != s.UsesAuth );
            settingsChanged |= ( usernameTextBox.Text != s.Username );
            settingsChanged |= ( passwordTextBox.Password != s.Password );

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
            shortcutTextBox.Text = shortcutText.ToString();

            this.UpdateUserDataFields();
        }
        #endregion

        /// <summary>
        /// Event handler for when the "apply" button had been pressed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event args.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // save settings
            SaveSettings();
            // and exit
            this.Close();
        } 
    }
}
