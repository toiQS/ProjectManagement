using PM.Domain;

namespace PM.DomainServices.Shared.DTOs
{
    public class ProjectDTO
    {
        public string Id { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerImage { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;
        public ProjectStatuses Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }
}
