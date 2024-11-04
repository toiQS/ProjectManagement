using System.Reflection;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;
using PM.WPF.ViewModels;
using PM.WPF.Views.Pages;
using Wpf.Ui;
using System.Windows;
using System.Windows.Input;

namespace PM.WPF.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MouseDown += MainWindow_MouseDown;
            MainPage.GotFocus += MainPage_GotFocus;

            var Data = new ViewModel();
            var user = Data.BasicListViewHeaders[0];
            

            var getFullPath = Assembly.GetExecutingAssembly().Location;
            var getProjectSpecificPath = getFullPath.Split("\\bin")[0];
            Logo.Source = new BitmapImage(new Uri($"{getProjectSpecificPath}\\Assets\\logo-2.png"));
            UserImage.ImageSource = new BitmapImage((new Uri($"{getProjectSpecificPath}{user.ImageUserPath}")));
        }
        private void MainPage_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // Hide the welcome message on first interaction
            WelcomeMessage.Visibility = Visibility.Collapsed;
            MainPage.SelectionChanged -= MainPage_SelectionChanged; // Detach after first trigger
        }
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WelcomeMessage.Visibility = Visibility.Collapsed;
            this.MouseDown -= MainWindow_MouseDown; // Detach after the first click
        }
        private void MainPage_GotFocus(object sender, RoutedEventArgs e)
        {
            // Hide the welcome message on first interaction
            WelcomeMessage.Visibility = Visibility.Collapsed;
            MainPage.GotFocus -= MainPage_GotFocus; // Detach after first trigger
        }

    }
}
