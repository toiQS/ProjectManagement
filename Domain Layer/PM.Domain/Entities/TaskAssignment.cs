namespace PM.Domain.Entities
{
    public class TaskAssignment
    {
        public string Id { get; set; } // Mã gán nhiệm vụ
        public string TaskId { get; set; } // Mã nhiệm vụ
        public string ProjectMemberId { get; set; } // Mã thành viên dự án

        public Task Task { get; set; } // Liên kết đến nhiệm vụ
        public ProjectMember ProjectMember { get; set; } // Liên kết đến thành viên dự án
    }
}
