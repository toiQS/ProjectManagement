using Shared.member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.task
{
    public class UpdateTask
    {
        public string TaskName { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty ;
        //public bool IsDone { get; set; }    
        public List<IndexMember> Members { get; set; }  = new List<IndexMember>();
    }
}
