using PM.WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.WPF.Models.member
{
    class MemberItem
    {
        public string Id { get; set; }
        public string MemberName { get; set; } // user name is here
        public string MemberPositionInProject { get; set; }
        public string MemberImage { get; set; } // image of user is here
        private readonly RelativePathConverter relativePathConverter = new RelativePathConverter();
        public MemberItem(string id, string memberName, string memberPositionInProject, string memberImage)
        {
            Id = id;
            MemberName = memberName;
            MemberPositionInProject = memberPositionInProject;
            MemberImage = relativePathConverter.Convert(memberImage).ToString() ?? string.Empty;
        }
    }
}
