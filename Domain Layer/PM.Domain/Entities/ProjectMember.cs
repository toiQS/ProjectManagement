namespace PM.Domain.Entities
{
    public class ProjectMember
    {
        public string Id { get; set; } // Mã thành viên dự án
        public string ProjectId { get; set; } // Mã dự án
        public string UserId { get; set; } // Mã người dùng
        public string RoleId { get; set; } // Vai trò trong dự án

        public Project Project { get; set; } // Liên kết đến dự án
        public User User { get; set; } // Liên kết đến người dùng
        public Role Role { get; set; } // Liên kết đến vai trò
    }
}
