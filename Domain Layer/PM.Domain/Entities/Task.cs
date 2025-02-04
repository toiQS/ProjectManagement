namespace PM.Domain.Entities
{
    public class Task
    {
        public string Id { get; set; } // Mã nhiệm vụ
        public string PlanId { get; set; } // Mã kế hoạch
        public string Name { get; set; } // Tên nhiệm vụ
        public string Description { get; set; } // Mô tả nhiệm vụ
        public DateTime StartDate { get; set; } // Ngày bắt đầu
        public DateTime EndDate { get; set; } // Ngày kết thúc
        public int StatusId { get; set; } // ID tình trạng
        public bool IsCompleted { get; set; } // Đã hoàn thành chưa

        public Plan Plan { get; set; } // Liên kết đến kế hoạch
        public Status Status { get; set; } // Liên kết đến tình trạng
        public ICollection<TaskAssignment> Assignments { get; set; } // Giao nhiệm vụ
    }
}
