using PM.WPF.Models.task;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.WPF.ViewModels
{
    public class TaskWindowViewModel
    {
        public ObservableCollection<TaskItem> TaskItemList { get; set; }
        public TaskWindowViewModel()
        {
            TaskItemList = new ObservableCollection<TaskItem>()
            {
                new TaskItem {Id = "1", EndAt = DateTime.Now.AddDays(10), StartAt = DateTime.Now, TaskName = "demo", TaskStatus ="done"}
            };
        }
    }
}
