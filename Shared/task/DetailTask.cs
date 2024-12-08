using Shared.member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.task
{
    public class DetailTask
    {
        public string TaskId { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty ;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public DateTime CreateAt { get; set; }
        public string Status { get; set; } = string.Empty;

        public bool IsDone { get; set; }
        public List<IndexMember> IndexMembers { get; set; } = new List<IndexMember>();
    }
}
