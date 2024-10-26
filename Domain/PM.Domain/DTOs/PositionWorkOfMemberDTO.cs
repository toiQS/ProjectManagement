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
        [Column(name:"Position In Project Id")]
        public string PostitionInProjectId { get; set; } = string.Empty;
        public virtual PostitionInProjectDTO PostitionInProject { get; set; }
        [Column(name: "Role Application User In Project Id")]

        [ForeignKey(nameof(RoleApplicationUserInProject))]
        public string RoleApplicationUserInProjectId { get; set; } = string.Empty;
        public virtual RoleApplicationUserInProjectDTO RoleApplicationUserInProject { get; set; }
    }
}
