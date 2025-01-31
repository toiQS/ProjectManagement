using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Domain
{
    [Table(name: "Member In Task")]
    public class MemberInTask
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        [ForeignKey(nameof(MemberProject))]
        [Column(name: "Member Id")]
        public string MemberInProjectId { get; set; } = string.Empty;
        public  MemberProject MemberProject { get; set; }
        [ForeignKey(nameof(TaskDTO))]
        [Column(name: "Task Id")]
        public string TaskId { get; set; } = string.Empty;
        public  TaskDTO TaskDTO { get; set; }
    }
}
