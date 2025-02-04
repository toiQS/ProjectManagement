using PM.WPF.ViewModels;
using PM.WPF.Views.Pages.Pages_of_ProjectWindow.TaskPages;
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
using Wpf.Ui.Controls;

namespace PM.WPF.Views.Windows
{
    /// <summary>
    /// Interaction logic for TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : FluentWindow
    {
        public TaskWindow()
        {
            InitializeComponent();
            AddContext();
        }
        public void AddContext()
        {
            DataContext = new TaskWindowViewModel();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NewTask newTask = new NewTask();
            newTask.Show();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateTask updateTask = new UpdateTask();
            updateTask.Show();
        }
    }
}
