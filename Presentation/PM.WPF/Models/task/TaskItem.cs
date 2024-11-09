using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.WPF.Models.task
{
    public class TaskItem
    {
        public string Id { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty;
        public string TaskStatus {  get; set; } = string.Empty;
        public DateTime StartAt {get; set; }
        public DateTime EndAt {get; set; }
    }
}
