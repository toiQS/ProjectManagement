using Shared.member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.task
{
    public class AddTask
    {
        public string TaskName { get; set; } = string.Empty;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<IndexMember> IndexMembers { get; set; } = new List<IndexMember>();
    }
}
