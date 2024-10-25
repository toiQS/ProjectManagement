using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.DTOs
{
    [Table(name:"Member In Task")]
    public class MemberInTaskDTO
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey(nameof(PositionWorkOfMember))]
        [Column(name:"Member Id")]
        public string PositionWorkOfMemberId { get; set; } = string.Empty;
        public virtual PositionWorkOfMemberDTO PositionWorkOfMember { get; set; }
        [ForeignKey(nameof(TaskDTO))]
        [Column(name:"Task Id")]
        public string TaskId { get; set; } = string.Empty;  
        public virtual TaskDTO TaskDTO { get; set; }
    }
}
