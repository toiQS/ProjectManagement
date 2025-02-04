﻿namespace PM.Domain.Entities
{
    public class Status
    {
        public int Id { get; set; } // Mã tình trạng
        public string Name { get; set; } // Tên tình trạng

        public ICollection<Project> Projects { get; set; } // Dự án có tình trạng này
        public ICollection<Plan> Plans { get; set; } // Kế hoạch có tình trạng này
        public ICollection<Task> Tasks { get; set; } // Nhiệm vụ có tình trạng này
    }
}
