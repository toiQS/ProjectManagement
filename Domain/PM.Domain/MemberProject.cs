using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Domain
{
    [Table(name: "Member In Project")]
    public class MemberProject
    {
        [Key]
        public string Id { get; set; }
       
        
        [ForeignKey(nameof(RoleInProject))]
        [Column(name: "Role In Project Id")]
        public string RoleInProjectId { get; set; } = string.Empty;
        public RoleInProject RoleInProject { get; set; }
        [ForeignKey(nameof(ApplicationUser))]
        [Column(name: "Application User Id")]
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; }
        [ForeignKey(nameof(PositionInProject))]
        public string PositionInProjectId {  get; set; } = string.Empty;
        public PositionInProject PositionInProject {  get; set; }
    }
}
