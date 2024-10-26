using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.DTOs
{
    [Table(name:"Task In Plan")]
    public class TaskInPlanDTO 
    {
        [Key]
        public string Id {  get; set; }
        [ForeignKey(nameof(Plan))]
        [Column(name: "Plan Id")]
        public string PlanId { get; set; } = string.Empty;
        public virtual PlanDTO Plan { get; set; }
        [ForeignKey(nameof(TaskDTO))]
        [Column(name:"Task Id")]
        public string TaskId { get; set; } = string.Empty;
        public virtual TaskDTO TaskDTO { get; set; }
    }
}
