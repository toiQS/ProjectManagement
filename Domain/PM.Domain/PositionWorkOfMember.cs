using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain
{
    [Table(name: "Position Work Of Member")]
    public class PositionWorkOfMember
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey(nameof(PositionInProject))]
        [Column(name: "Position In Project Id")]
        public string PostitionInProjectId { get; set; } = string.Empty;
        public virtual PositionInProject PositionInProject { get; set; }
        [Column(name: "Role Application User In Project Id")]

        [ForeignKey(nameof(RoleApplicationUserInProject))]
        public string RoleApplicationUserInProjectId { get; set; } = string.Empty;
        public virtual RoleApplicationUserInProject RoleApplicationUserInProject { get; set; }
    }
}
