using Shared.task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.member
{
    public class IndexMember
    {
        public string RoleUserInProjectId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PositionWorkName { get; set; } = string.Empty;
        public string UserAvata { get; set; } = string.Empty;
        public List<IndexTask> Tasks { get; set; } = new List<IndexTask>();
    }
}
