using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain
{
    [Table(name: "Position In Project")]
    public class PostitionInProject
    {
        [Key]
        public string Id { get; set; }
        [Column(name: "Position Name")]
        public string PositionName { get; set; } = string.Empty;
        [Column(name: "Position Description")]
        public string PositionDescription { get; set; } = string.Empty;
        [ForeignKey(nameof(Project))]
        [Column(name: "Project Id")]
        public string ProjectId { get; set; } = string.Empty;
        public virtual Project Project { get; set; }
    }
}
