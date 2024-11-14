using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.WPF.Models.plan
{
    public class InfoDetailPlan
    {
        public string PlanName { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }
}
