using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.project
{
    public class AddProject
    {
        public string ProjectName { get; set; } = string.Empty;
        public DateTime StartAt { get; set; } = DateTime.Now;
        public DateTime EndAt { get; set; } = DateTime.Now.AddYears(1);
        public string ProjectDescription { get; set; } = string.Empty;
    }
}
