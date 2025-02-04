namespace PM.Domain.Entities
{
    public class User
    {
        public string Id { get; set; } // Mã người dùng
        public string FirstName { get; set; } // Họ
        public string LastName { get; set; } // Tên
        public string Nickname { get; set; } // Biệt danh
        public string Email { get; set; } // Email
        public string PhoneNumber { get; set; } // Số điện thoại
        public string AvatarPath { get; set; } // Đường dẫn ảnh đại diện

        public ICollection<ProjectMember> ProjectMemberships { get; set; } // Các dự án tham gia
        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }
}
