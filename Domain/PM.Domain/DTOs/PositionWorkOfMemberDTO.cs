using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.DTOs
{
    [Table(name:"Position Work Of Member")]
    public class PositionWorkOfMemberDTO 
    {
        [Key]
        public string Id {  get; set; }
        [ForeignKey(nameof(PositionWorkOfMemberDTO))]
        [Column(name:"Position Id")]
        public string PositionId { get; set; } = string.Empty;
        public virtual PostitionInProjectDTO PostitionInProject { get; set; }
        [Column(name:"User Id")]

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; }
    }
}
