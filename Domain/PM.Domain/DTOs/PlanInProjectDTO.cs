using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.DTOs
{
    [Table(name:"Plan In Project")]
    public class PlanInProjectDTO 
    {
        [Key]
        public string Id {  get; set; }
        [ForeignKey(nameof(Project))]
        [Column(name:"Project Id")]
        public string ProjectId { get; set; } = string.Empty;
        public virtual ProjectDTO Project { get; set; }
        [ForeignKey(nameof(Plan))]
        [Column(name:"Plan Id")]
        public string PlanId { get; set; } = string.Empty;
        public virtual PlanDTOs Plan { get; set; }
    }
}
