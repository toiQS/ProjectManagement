using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.WPF.Models.projects
{
    public class ProjectDashBoard
    {
        public string ProjectName { get; set; } = string.Empty;
        public List<Dictionary<string, object>> ProjectMember {  get; set; }
        public double PercentProgress { get; set; }
        public string ProjectDescription { get; set; } = string.Empty;
        public string ProjectOwner {  get; set; } = string.Empty;
        public string ProjectStatus {  get; set; } = string.Empty;
    }
}
