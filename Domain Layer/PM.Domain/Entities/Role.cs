﻿namespace PM.Domain.Entities
{
    public class Role
    {
        public string Id { get; set; } // Mã vai trò
        public string Name { get; set; } // Tên vai trò
        public string Description { get; set; } // Mô tả vai trò

        public ICollection<ProjectMember> ProjectMembers { get; set; } // Thành viên có vai trò này
    }
}
