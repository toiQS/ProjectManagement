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

namespace PM.WPF.Views.Pages.Pages_of_MainWindow
{
    /// <summary>
    /// Interaction logic for SettingPage.xaml
    /// </summary>
    public partial class SettingPage : Page
    {
        public SettingPage()
        {
            InitializeComponent();
        }
        private void ThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedTheme = (e.AddedItems[0] as ComboBoxItem)?.Tag.ToString();
                if (selectedTheme == "Light")
                {
                    Application.Current.Resources["AppTheme"] = new Uri("Themes/LightTheme.xaml", UriKind.Relative);
                }
                else if (selectedTheme == "Dark")
                {
                    Application.Current.Resources["AppTheme"] = new Uri("Themes/DarkTheme.xaml", UriKind.Relative);
                }
            }
        }
        private void FontSizeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Application.Current.Resources["BaseFontSize"] = e.NewValue;
        }

        private void ColorSchemeChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedColor = (e.AddedItems[0] as ComboBoxItem)?.Tag.ToString();
            switch (selectedColor)
            {
                case "Blue":
                    Application.Current.Resources["PrimaryColor"] = new SolidColorBrush(Colors.Blue);
                    break;
                case "Green":
                    Application.Current.Resources["PrimaryColor"] = new SolidColorBrush(Colors.Green);
                    break;
                case "Red":
                    Application.Current.Resources["PrimaryColor"] = new SolidColorBrush(Colors.Red);
                    break;
                default:
                    Application.Current.Resources["PrimaryColor"] = new SolidColorBrush(Colors.Gray);
                    break;
            }
        }

    }
}
