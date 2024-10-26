using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.DTOs
{
    [Table(name:"Role Application User In Project")]
    public class RoleApplicationUserInProjectDTO
    {
        [Key]
        public string Id {  get; set; }
        [ForeignKey(nameof(ProjectDTO))]
        [Column(name:"Project Id")]
        public string ProjectId { get; set; } = string.Empty;
        public virtual ProjectDTO ProjectDTO { get; set; }
        [ForeignKey(nameof(RoleInProject))]
        [Column(name:"Role In Project Id")]
        public string RoleInProjectId { get; set; } = string.Empty;
        public virtual RoleInProjectDTO RoleInProject { get; set; }
        [ForeignKey(nameof(ApplicationUser))]
        [Column(name:"Application User Id")]
        public string ApplicationUserId { get; set; } = string.Empty;
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
