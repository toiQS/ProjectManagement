using PM.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PM.WPF.ViewModels
{
    public class ProjectViewModel : ViewModelBase
    {
        private Project _selectedProject;

        public ObservableCollection<Project> Projects { get; set; }

        public Project SelectedProject
        {
            get => _selectedProject;
            set => SetProperty(ref _selectedProject, value);
        }

        public ProjectViewModel()
        {
            Projects = new ObservableCollection<Project>
        {
            new Project { Name = "Dự án A", Description = "Mô tả A", Status = "In Progress", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1) },
            new Project { Name = "Dự án B", Description = "Mô tả B", Status = "Completed", StartDate = DateTime.Now.AddMonths(-2), EndDate = DateTime.Now.AddMonths(-1) }
        };
        }
    }

}
