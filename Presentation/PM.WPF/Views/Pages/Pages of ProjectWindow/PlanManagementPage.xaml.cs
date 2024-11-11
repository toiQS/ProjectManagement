using PM.WPF.Models.plan;
using PM.WPF.ViewModels;
using PM.WPF.Views.Windows;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
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
using Wpf.Ui.Controls;

namespace PM.WPF.Views.Pages.Pages_of_ProjectWindow
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

        private void PlanItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(PlanItemListView.SelectedItem is PlanItem planItem)
            {
                TaskWindow taskWindow = new TaskWindow();
                taskWindow.Show();
            }
        }
    }
}
