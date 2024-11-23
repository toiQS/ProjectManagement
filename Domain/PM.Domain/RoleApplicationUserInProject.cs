using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain
{
    [Table(name: "Role Application User In Project")]
    public class RoleApplicationUserInProject
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey(nameof(Project))]
        [Column(name: "Project Id")]
        public string ProjectId { get; set; } = string.Empty;
        public virtual Project Project { get; set; }
        [ForeignKey(nameof(RoleInProject))]
        [Column(name: "Role In Project Id")]
        public string RoleInProjectId { get; set; } = string.Empty;
        public virtual RoleInProject RoleInProject { get; set; }
        [ForeignKey(nameof(ApplicationUser))]
        [Column(name: "Application User Id")]
        public string ApplicationUserId { get; set; } = string.Empty;
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
