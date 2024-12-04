using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    internal class MemberModel
    {
    }
    public class MemberIndex
    {
        public string Id { get; set; } = string.Empty;
        public string PositionWorkId { get; set; } = string.Empty ;
        public string MemberName { get; set; } = string.Empty; // user name is here
        //public string MemberPositionInProject { get; set; } = string.Empty;
        public string MemberImage { get; set; } = string.Empty; // image of user is here
        public string PositionNameInProject { get; set; } = string.Empty;
    }
    //public class NewMember
    //{
    //    public 
    //}
}
