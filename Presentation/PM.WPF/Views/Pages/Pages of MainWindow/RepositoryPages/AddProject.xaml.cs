using Microsoft.Win32;
using Syncfusion.DocIO.DLS;
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

namespace PM.WPF.Views.Pages.Pages_of_MainWindow.RepositoryPages
{
    /// <summary>
    /// Interaction logic for AddProject.xaml
    /// </summary>
    public partial class AddProject : FluentWindow
    {
        public AddProject()
        {
            InitializeComponent();
        }

        private void ChooseImage_Click(object sender, RoutedEventArgs e)
        {
            // Tạo hộp thoại mở tệp
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Choose Project Image",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*"
            };

            // Hiển thị hộp thoại và kiểm tra nếu người dùng đã chọn tệp
            if (openFileDialog.ShowDialog() == true)
            {
                // Lấy đường dẫn của tệp được chọn
                string selectedFilePath = openFileDialog.FileName;
                ShowPath.Text = selectedFilePath;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
