using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class TaskModel
    {
    }
    public class TaskIndex
    {
        public string Id { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty;
        public string TaskStatus { get; set; } = string.Empty;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }
    public class NewTask
    {
        public string TaskName { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public int Status { get; set; }
        public List<MemberIndex> Members { get; set; } = new List<MemberIndex>();
    }
    public enum TaskStatuses
    {
        Node,
        Waiting,
        Processing,
        Completed
    };
    public class TaskDetail
    {
        public string Id { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public string TaskStatus { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public List<MemberIndex> Members { get; set; } = new List<MemberIndex> { };
    }
    public class TaskUpdate
    {
        public string TaskName { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public string TaskStatus { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public List<MemberIndex> Members { get; set; } = new List<MemberIndex> { };
    }
}
