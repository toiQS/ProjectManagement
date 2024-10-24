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
    [Table(name:"Role In Project")]
    public class RoleInProjectDTO 
    {
        [Key]
        public string Id { get; set; }
        [Column(name: "Role Name")]
        public string RoleName { get; set; } = string.Empty;
        [Column(name:"Role Description")]
        public string RoleDescription { get; set; } = string.Empty;
    }
}
