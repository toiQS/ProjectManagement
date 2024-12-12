using PM.WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.WPF.Models.projects
{
    public class InfoProject
    {
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectDescription {  get; set; } = string.Empty;
        public string ProjectVersion { get; set; } = string.Empty;
        public string ProjectOwner {  get; set; } = string.Empty;
        public string ProjectImage {  get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public string IsAccessed { get; set; } = string.Empty;
        private readonly RelativePathConverter relativePathConverter = new RelativePathConverter();
        public InfoProject(string projectName, string projectDescription, string projectVersion, string projectOwner, string projectImage, 
            DateTime createAt, string isAccessed)
        {
            ProjectName = projectName;
            ProjectDescription = projectDescription;
            ProjectVersion = projectVersion;
            ProjectOwner = projectOwner;
            ProjectImage = relativePathConverter.Convert(projectImage).ToString() ?? string.Empty;
            CreateAt = createAt;
            IsAccessed = isAccessed;
        }
    }
}
