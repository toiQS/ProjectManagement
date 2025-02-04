namespace PM.Domain.Entities
{
    public class Plan
    {
        public string Id { get; set; } // Mã kế hoạch
        public string ProjectId { get; set; } // Mã dự án
        public string Name { get; set; } // Tên kế hoạch
        public DateTime StartDate { get; set; } // Ngày bắt đầu
        public DateTime EndDate { get; set; } // Ngày kết thúc
        public int StatusId { get; set; } // ID tình trạng
        public bool IsCompleted { get; set; } // Đã hoàn thành chưa

        public Project Project { get; set; } // Liên kết đến dự án
        public Status Status { get; set; } // Liên kết đến tình trạng
        public ICollection<Task> Tasks { get; set; } // Các nhiệm vụ
    }
}
