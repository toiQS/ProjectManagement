using PM.WPF.ViewModels;
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
    }
}
