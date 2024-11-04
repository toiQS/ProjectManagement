using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.WPF.Models
{
    public class Project
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }

        // List of tasks related to this project (optional)
        public ObservableCollection<Task> Tasks { get; set; }

        // List of members (optional)
        public ObservableCollection<Member> Members { get; set; }

        public Project()
        {
            Tasks = new ObservableCollection<Task>();
            Members = new ObservableCollection<Member>();
        }
    }

}
