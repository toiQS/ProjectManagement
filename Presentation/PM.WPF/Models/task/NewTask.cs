namespace PM.WPF.Models.task
{
    public class NewTask
    {
        public string TaskName { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public List<Member> MemberIds { get; set; }
    }
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
