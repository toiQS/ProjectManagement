using PM.WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.WPF.Models.projects
{
    public class ProjectItem
    {
        public string Id { get; set; }
        public string ProjectName {  get; set; }
        public string ProjectOwner { get; set; }
        public string ProjectImage { get; set; }
        private readonly RelativePathConverter relativePathConverter = new RelativePathConverter();
        public ProjectItem (string id,string projectName, string projectOwner, string projectImage)
        {
            Id = id;
            this.ProjectName = projectName;
            this.ProjectOwner = projectOwner;
            this.ProjectImage = relativePathConverter.Convert(projectImage).ToString() ?? string.Empty;
        }
    }
}
