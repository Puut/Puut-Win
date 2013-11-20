using MahApps.Metro.Controls;
using Puut.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Puut
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class PreferencesWindow : MetroWindow
    {
        public PreferencesWindow()
        {
            InitializeComponent();

            LoadSettings();

            UpdateUserDataFields();
        }

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

        private void shortcutTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void shortcutTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // The text box grabs all input.
            e.Handled = true;

            // Fetch the actual shortcut key.
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);

            // Ignore modifier keys.
            if (key == Key.LeftShift || key == Key.RightShift
                || key == Key.LeftCtrl || key == Key.RightCtrl
                || key == Key.LeftAlt || key == Key.RightAlt
                || key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            // Build the shortcut key name.
            StringBuilder shortcutText = new StringBuilder();
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                shortcutText.Append("Ctrl+");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                shortcutText.Append("Shift+");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0)
            {
                shortcutText.Append("Alt+");
            }
            shortcutText.Append(key.ToString());

            // Update the text box.
            shortcutTextBox.Text = shortcutText.ToString();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateUserDataFields();
        }

        private void usesAuthCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateUserDataFields();
        }

        private void UpdateUserDataFields()
        {
            bool status = (usesAuthCheckbox.IsChecked.Value);

            usernameTextBox.IsEnabled = passwordTextBox.IsEnabled = status;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            this.Close();
        }

        
    }
}
