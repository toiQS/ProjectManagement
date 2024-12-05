using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [ForeignKey(nameof(Status))]
        public int StatusId { get; set; }
        public Status Status { get; set; }
        [Column(name: "Create At")]
        public DateTime CreateAt { get; set; }
        [Column(name: "Start At")]
        public DateTime StartAt { get; set; }
        [Column(name: "End At")]
        public DateTime EndAt { get; set; }
        [Column(name: " Is Done")]
        public bool IsDone { get; set; }
    }
   
}
