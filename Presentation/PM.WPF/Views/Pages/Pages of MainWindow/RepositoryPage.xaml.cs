using PM.WPF.ViewModels;
using PM.WPF.Views.Pages.Pages_of_MainWindow.RepositoryPages;
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
    /// Interaction logic for RepositoryPage.xaml
    /// </summary>
    public partial class RepositoryPage : Page
    {
        public RepositoryPage()
        {
            InitializeComponent();
            this.DataContext = new HomePageViewModel();
        }

        private void ProjectListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddProject addProject = new AddProject();
            addProject.Show();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
