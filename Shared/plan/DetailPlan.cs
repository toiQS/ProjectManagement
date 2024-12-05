using Shared.task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.plan
{
    public class DetailPlan
    {
        public string PlanId { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsDone { get; set; }

        public List<IndexTask> Tasks { get; set; } = new List<IndexTask>(); 
    }
}
