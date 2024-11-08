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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PM.WPF.Views.Pages
{
    /// <summary>
    /// Interaction logic for PlanManagementPage.xaml
    /// </summary>
    public partial class PlanManagementPage : Page
    {
        public PlanManagementPage()
        {
            InitializeComponent();

            DataContext = new PlanViewModel();
            AddContextConponent();
        }
        public void AddContextConponent()
        {
            PlanNameText.Text = "Member";
        }
    }
}
