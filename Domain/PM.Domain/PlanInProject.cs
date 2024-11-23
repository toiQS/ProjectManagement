using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain
{
    [Table(name: "Plan In Project")]
    public class PlanInProject
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey(nameof(Project))]
        [Column(name: "Project Id")]
        public string ProjectId { get; set; } = string.Empty;
        public virtual Project Project { get; set; }
        [ForeignKey(nameof(Plan))]
        [Column(name: "Plan Id")]
        public string PlanId { get; set; } = string.Empty;
        public virtual Plan Plan { get; set; }
    }
}
