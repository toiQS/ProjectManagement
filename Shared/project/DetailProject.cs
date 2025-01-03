using Shared.member;
using Shared.plan;
using Shared.task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.project
{
    public class DetailProject
    {
        public string ProjectId { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAccessed { get; set; }
        public bool IsDone { get; set; }
        public string OwnerName { get; set; } = string.Empty; //user name 
        public string OwnerAvata { get; set; } = string.Empty;
        public List<IndexPlan> Plans { get; set; } = new List<IndexPlan>();
        public List<IndexMember> Members { get; set; } = new List<IndexMember> { };
        public string Status { get; set; } = string.Empty;
        public int QuantityMember { get; set; } = 0;
        public string ProjectDescription {  get; set; } = string.Empty;
    }
}
