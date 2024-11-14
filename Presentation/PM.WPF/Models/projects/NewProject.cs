using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.WPF.Models.projects
{
    public class NewProject
    {
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectDescription {  get; set; } = string.Empty;
        public string ProjectVersion {  get; set; } = string.Empty;
        public string ProjectImage { get; set; } = string.Empty;
    }
}
