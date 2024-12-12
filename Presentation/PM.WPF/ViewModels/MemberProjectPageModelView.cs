using PM.WPF.Models.member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.WPF.ViewModels
{
    class MemberProjectPageModelView
    {
        public List<MemberItem> MemberItemList { get; set; }
        public MemberProjectPageModelView()
        {
            MemberItemList = new List<MemberItem>()
            {
                new MemberItem ( "1","demo1", "demo","\\Assets\\background.jpg")
            };
        }
    }
}
