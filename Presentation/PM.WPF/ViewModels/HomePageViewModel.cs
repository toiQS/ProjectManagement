using PM.WPF.Models.projects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.WPF.ViewModels
{
    public class HomePageViewModel
    {
        public ObservableCollection<ProjectItem> ProjectItemList { get; set; }
        public HomePageViewModel()
        {
            ProjectItemList = new ObservableCollection<ProjectItem>()
            {
                new ProjectItem("1","project demo","John Doe", "\\Assets\\background.jpg")
            };
        }
    }
}
