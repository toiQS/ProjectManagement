using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.WPF.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // List of tasks assigned to this member (optional)
        public ObservableCollection<Task> AssignedTasks { get; set; }

        public Member()
        {
            AssignedTasks = new ObservableCollection<Task>();
        }
    }
}
