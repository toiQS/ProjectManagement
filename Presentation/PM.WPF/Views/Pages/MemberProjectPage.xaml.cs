using PM.WPF.Models;
using PM.WPF.Models.member;
using PM.WPF.ViewModels;
using PM.WPF.Views.Windows;
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
    /// Interaction logic for MemberProjectPage.xaml
    /// </summary>
    public partial class MemberProjectPage : Page
    {
        public MemberProjectPage()
        {
            InitializeComponent();

            ProjectNameText.Text = "Project Name";
            DataContext = new MemberProjectPageModelView();
        }

        private void MemberInProjectList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MemberInProjectList.SelectedItem is MemberItem member )
            {
                App.Current.MainWindow.Hide();
                UserWindow userWindow = new UserWindow();
                userWindow.ShowDialog();
                
            }
        }
    }
}
