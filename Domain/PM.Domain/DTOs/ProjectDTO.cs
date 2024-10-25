using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
#region design project entity
namespace PM.Domain.DTOs
{
    [Table(name:"Project")]
    public class ProjectDTO 
    {
        [Key]
        public string Id { get; set; }  
        [Column(name: "Project Name")]
        public string ProjectName { get; set; } = string.Empty;
        [Column(name:"Project Description")]
        public string ProjectDescription { get; set; } = string.Empty;
        [Column(name: "Project Version")]
        public string ProjectVersion { get; set; } = string.Empty;
        [Column(name: "Project Status")]
        public string Projectstatus { get; set; } = string.Empty;
        [Column(name: "Create At")]
        public string CreateAt { get; set; }
        [Column(name:"Is Deleted")]
        public bool IsDeleted { get; set; }
        [Column(name:"Is Accessed")]
        public bool IsAccessed { get; set; }
    }
}
#endregion