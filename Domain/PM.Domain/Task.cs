using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain
{
    [Table(name: "Task")]
    public class TaskDTO
    {
        [Key]
        public string Id { get; set; }
        [Column(name: "Task Name")]
        public string TaskName { get; set; } = string.Empty;
        [Column(name: "Task Description")]
        public string TaskDescription { get; set; } = string.Empty;
        [Column(name: "Task Status")]
        public string TaskStatus { get; set; } = string.Empty;
        [Column(name: "Create At")]
        public DateTime CreateAt { get; set; }
        [Column(name: "Start At")]
        public DateTime StartAt { get; set; }
        [Column(name: "End At")]
        public DateTime EndAt { get; set; }
    }
}
