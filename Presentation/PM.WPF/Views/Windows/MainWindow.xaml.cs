using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;
using PM.WPF.ViewModels;
using PM.WPF.Views.Pages;
using System.Windows.Controls;

namespace PM.WPF.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow
    {
        private bool isHomePageActive = false;
        public MainWindow()
        {
            InitializeComponent();

            // Set up initial events
            this.MouseDown += MainWindow_MouseDown;
            MainPage.GotFocus += MainPage_GotFocus;
            MainPage.SelectionChanged += MainPage_SelectionChanged;
            MainPage.SelectionChanged += MainPage_SelectionChanged;
            MainPage.GotFocus += MainPage_GotFocus;

            // Load resources and initialize UI elements
            InitializeLogoAndUserImage();
        }

        // Method to initialize logo and user image sources
        private void InitializeLogoAndUserImage()
        {
            // Retrieve project path
            var getFullPath = Assembly.GetExecutingAssembly().Location;
            var getProjectSpecificPath = getFullPath.Split("\\bin")[0];

            // Load the ViewModel data
            var viewModel = new ViewModel();
            var user = viewModel.BasicListViewHeaders[0];

            // Set Logo and User Image sources
            Logo.Source = new BitmapImage(new Uri($"{getProjectSpecificPath}\\Assets\\logo-2.png"));
            UserImage.ImageSource = new BitmapImage(new Uri($"{getProjectSpecificPath}{user.ImageUserPath}"));
        }

        // Event handler for hiding the welcome message when an item in NavigationView is selected
        private void MainPage_SelectionChanged(object sender, RoutedEventArgs e)
        {
            WelcomeMessage.Visibility = Visibility.Collapsed;
            MainPage.SelectionChanged -= MainPage_SelectionChanged; // Unsubscribe after first trigger
        }

        // Event handler for hiding the welcome message when window is clicked
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WelcomeMessage.Visibility = Visibility.Collapsed;
            this.MouseDown -= MainWindow_MouseDown; // Unsubscribe after the first click
        }

        // Event handler for hiding the welcome message when NavigationView gains focus
        private void MainPage_GotFocus(object sender, RoutedEventArgs e)
        {
            WelcomeMessage.Visibility = Visibility.Collapsed;
            MainPage.GotFocus -= MainPage_GotFocus; // Unsubscribe after first trigger
        }
        private void MainPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainPage.SelectedItem is NavigationViewItem selectedItem && selectedItem.Content.ToString() == "Home")
            {
                isHomePageActive = true;
                WelcomeMessage.Visibility = Visibility.Collapsed;
            }
            else
            {
                isHomePageActive = false;
            }
        }

    }
}
