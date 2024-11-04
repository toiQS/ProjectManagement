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
using System.Windows.Shapes;

namespace PM.WPF.Views.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void CanelButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ShowWindow<MainWindow>(() => new MainWindow());
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ShowWindow<RegisterWindow>(() => new RegisterWindow());
        }
        private void ShowWindow<T>(Func<T> windowConstructor) where T : Window
        {
            try
            {
                var newWindow = windowConstructor.Invoke();
                newWindow.Owner = this;
                newWindow.Closed += (s, args) => ShowMainWindow(); // Shows main window after child window closes
                Hide(); // Hides the main window while child window is open
                newWindow.Show(); // Shows the child window
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Shows the main window again after another window is closed.
        /// </summary>
        private void ShowMainWindow()
        {
            Show(); // Shows the main window
        }
    }
}
