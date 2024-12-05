using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.plan
{
    public class AddPlan
    {
        public string Name { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt {  get; set; }
    }
}
