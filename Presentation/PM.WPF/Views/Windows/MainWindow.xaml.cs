using System.Reflection;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;
using PM.WPF.ViewModels;
using PM.WPF.Views.Pages;
using Wpf.Ui;
using System.Windows;

namespace PM.WPF.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow
    {
        public MainWindow()
        {
            var Data = new ViewModel();
            var user = Data.BasicListViewHeaders[0];
            InitializeComponent();
            var getFullPath = Assembly.GetExecutingAssembly().Location;
            var getProjectSpecificPath = getFullPath.Split("\\bin")[0];
            Logo.Source = new BitmapImage(new Uri($"{getProjectSpecificPath}\\Assets\\logo-2.png"));
            UserImage.ImageSource = new BitmapImage((new Uri($"{getProjectSpecificPath}{user.ImageUserPath}")));
        }
    }
}
