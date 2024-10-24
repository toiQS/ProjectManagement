using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PM.Domain.DTOs;

namespace PM.Persistence.Context
{
    #region register entities and seeding data
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<string>,string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
           
        }
        #region register entities
        public DbSet<ApplicationUser> ApplicationUser {  get; set; }
        public DbSet<MemberInTaskDTO> MemberInTask { get; set; }
        public DbSet<PlanDTOs> PlanDTO { get; set; }
        public DbSet<PlanInProjectDTO> PlanInProject { get; set; }
        public DbSet<PositionWorkOfMemberDTO> PositionWorkOfMember { get; set; }
        public DbSet<PostitionInProjectDTO> PostitionInProject { get; set; }
        public DbSet<ProjectDTO> ProjectDTO { get; set; }
        public DbSet<RoleApplicationUserInProjectDTO> RoleApplicationUserInProject { get; set; }
        public DbSet<RoleInProjectDTO> RoleInProject { get; set; }
        public DbSet<TaskDTO> TaskDTO { get; set; }
        public DbSet<TaskInPlanDTO> TaskInPlan { get; set; }
        #endregion

        #region seeding data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Remove the AspNet prefix from table names
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName != null && tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            //modelBuilder.Entity<PositionWorkOfMemberDTO>()
            //.HasOne(p => p.User)
            //.WithMany()
            //.HasForeignKey(p => p.UserId)
            //.OnDelete(DeleteBehavior.Restrict); // Sử dụng Restrict thay cho Cascade


            // Seed data for ApplicationUser
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser { Id = "1", FirstName = "John", LastName = "Doe", FullName = "John Doe", Phone = "123456789", PathImage = "john.jpg", UserName = "john_doe", Email = "john.doe@example.com" },
                new ApplicationUser { Id = "2", FirstName = "Jane", LastName = "Smith", FullName = "Jane Smith", Phone = "987654321", PathImage = "jane.jpg", UserName = "jane_smith", Email = "jane.smith@example.com" },
                new ApplicationUser { Id = "3", FirstName = "Michael", LastName = "Johnson", FullName = "Michael Johnson", Phone = "234567890", PathImage = "michael.jpg", UserName = "michael_j", Email = "michael.j@example.com" },
                new ApplicationUser { Id = "4", FirstName = "Emily", LastName = "Davis", FullName = "Emily Davis", Phone = "345678901", PathImage = "emily.jpg", UserName = "emily_d", Email = "emily.d@example.com" },
                new ApplicationUser { Id = "5", FirstName = "Chris", LastName = "Brown", FullName = "Chris Brown", Phone = "456789012", PathImage = "chris.jpg", UserName = "chris_b", Email = "chris.b@example.com" }
            );

            // Seed data for ProjectDTO
            modelBuilder.Entity<ProjectDTO>().HasData(
                new ProjectDTO { Id = "1", ProjectName = "Project Alpha", ProjectDescription = "First project description", ProjectVersion = "1.0", Projectstatus = "In Progress", CreateAt = DateTime.Now.ToString(), IsDeleted = false },
                new ProjectDTO { Id = "2", ProjectName = "Project Beta", ProjectDescription = "Second project description", ProjectVersion = "1.1", Projectstatus = "Completed", CreateAt = DateTime.Now.ToString(), IsDeleted = false },
                new ProjectDTO { Id = "3", ProjectName = "Project Gamma", ProjectDescription = "Third project description", ProjectVersion = "1.2", Projectstatus = "On Hold", CreateAt = DateTime.Now.ToString(), IsDeleted = false },
                new ProjectDTO { Id = "4", ProjectName = "Project Delta", ProjectDescription = "Fourth project description", ProjectVersion = "1.3", Projectstatus = "Cancelled", CreateAt = DateTime.Now.ToString(), IsDeleted = false },
                new ProjectDTO { Id = "5", ProjectName = "Project Epsilon", ProjectDescription = "Fifth project description", ProjectVersion = "2.0", Projectstatus = "In Progress", CreateAt = DateTime.Now.ToString(), IsDeleted = false }
            );

            // Seed data for TaskDTO
            modelBuilder.Entity<TaskDTO>().HasData(
                new TaskDTO { Id = "1", TaskName = "Task 1", TaskDescription = "Task description for project 1", TaskStatus = "Pending", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(1), EndAt = DateTime.Now.AddDays(5) },
                new TaskDTO { Id = "2", TaskName = "Task 2", TaskDescription = "Task description for project 2", TaskStatus = "Completed", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(2), EndAt = DateTime.Now.AddDays(6) },
                new TaskDTO { Id = "3", TaskName = "Task 3", TaskDescription = "Task description for project 3", TaskStatus = "In Progress", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(3), EndAt = DateTime.Now.AddDays(7) },
                new TaskDTO { Id = "4", TaskName = "Task 4", TaskDescription = "Task description for project 4", TaskStatus = "On Hold", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(4), EndAt = DateTime.Now.AddDays(8) },
                new TaskDTO { Id = "5", TaskName = "Task 5", TaskDescription = "Task description for project 5", TaskStatus = "Pending", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(5), EndAt = DateTime.Now.AddDays(9) }
            );

            // Seed data for PlanDTOs
            modelBuilder.Entity<PlanDTOs>().HasData(
                new PlanDTOs { Id = "1", PlanName = "Plan A", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(1), EndAt = DateTime.Now.AddMonths(1) },
                new PlanDTOs { Id = "2", PlanName = "Plan B", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(2), EndAt = DateTime.Now.AddMonths(2) },
                new PlanDTOs { Id = "3", PlanName = "Plan C", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(3), EndAt = DateTime.Now.AddMonths(3) },
                new PlanDTOs { Id = "4", PlanName = "Plan D", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(4), EndAt = DateTime.Now.AddMonths(4) },
                new PlanDTOs { Id = "5", PlanName = "Plan E", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(5), EndAt = DateTime.Now.AddMonths(5) }
            );

            // Seed data for TaskInPlanDTO
            modelBuilder.Entity<TaskInPlanDTO>().HasData(
                new TaskInPlanDTO { Id = "1", PlanId = "1", TaskId = "1" },
                new TaskInPlanDTO { Id = "2", PlanId = "2", TaskId = "2" },
                new TaskInPlanDTO { Id = "3", PlanId = "3", TaskId = "3" },
                new TaskInPlanDTO { Id = "4", PlanId = "4", TaskId = "4" },
                new TaskInPlanDTO { Id = "5", PlanId = "5", TaskId = "5" }
            );

            // Seed data for MemberInTaskDTO
            modelBuilder.Entity<MemberInTaskDTO>().HasData(
                new MemberInTaskDTO { Id = "1", MemberId = "1", TaskId = "1" },
                new MemberInTaskDTO { Id = "2", MemberId = "2", TaskId = "2" },
                new MemberInTaskDTO { Id = "3", MemberId = "3", TaskId = "3" },
                new MemberInTaskDTO { Id = "4", MemberId = "4", TaskId = "4" },
                new MemberInTaskDTO { Id = "5", MemberId = "5", TaskId = "5" }
            );

            // Seed data for PostitionInProjectDTO
            modelBuilder.Entity<PostitionInProjectDTO>().HasData(
                new PostitionInProjectDTO { Id = "1", PositionName = "Manager", PositionDescription = "Project Manager", ProjectId = "1" },
                new PostitionInProjectDTO { Id = "2", PositionName = "Developer", PositionDescription = "Software Developer", ProjectId = "2" },
                new PostitionInProjectDTO { Id = "3", PositionName = "Tester", PositionDescription = "Software Tester", ProjectId = "3" },
                new PostitionInProjectDTO { Id = "4", PositionName = "Designer", PositionDescription = "UI/UX Designer", ProjectId = "4" },
                new PostitionInProjectDTO { Id = "5", PositionName = "DevOps", PositionDescription = "DevOps Engineer", ProjectId = "5" }
            );

            // Seed data for RoleInProjectDTO
            modelBuilder.Entity<RoleInProjectDTO>().HasData(
                new RoleInProjectDTO { Id = "1", RoleName = "Admin", RoleDescription = "Administrator Role" },
                new RoleInProjectDTO { Id = "2", RoleName = "Contributor", RoleDescription = "Contributor Role" },
                new RoleInProjectDTO { Id = "3", RoleName = "Viewer", RoleDescription = "Viewer Role" },
                new RoleInProjectDTO { Id = "4", RoleName = "Editor", RoleDescription = "Editor Role" },
                new RoleInProjectDTO { Id = "5", RoleName = "Owner", RoleDescription = "Owner Role" }
            );

            // Seed data for RoleApplicationUserInProjectDTO
            modelBuilder.Entity<RoleApplicationUserInProjectDTO>().HasData(
                new RoleApplicationUserInProjectDTO { Id = "1", ProjectId = "1", RoleId = "1" },
                new RoleApplicationUserInProjectDTO { Id = "2", ProjectId = "2", RoleId = "2" },
                new RoleApplicationUserInProjectDTO { Id = "3", ProjectId = "3", RoleId = "3" },
                new RoleApplicationUserInProjectDTO { Id = "4", ProjectId = "4", RoleId = "4" },
                new RoleApplicationUserInProjectDTO { Id = "5", ProjectId = "5", RoleId = "5" }
            );

            // Seed data for PlanInProjectDTO
            modelBuilder.Entity<PlanInProjectDTO>().HasData(
                new PlanInProjectDTO { Id = "1", ProjectId = "1", PlanId = "1" },
                new PlanInProjectDTO { Id = "2", ProjectId = "2", PlanId = "2" },
                new PlanInProjectDTO { Id = "3", ProjectId = "3", PlanId = "3" },
                new PlanInProjectDTO { Id = "4", ProjectId = "4", PlanId = "4" },
                new PlanInProjectDTO { Id = "5", ProjectId = "5", PlanId = "5" }
            );

            // Seed data for PositionWorkOfMemberDTO
            modelBuilder.Entity<PositionWorkOfMemberDTO>().HasData(
                new PositionWorkOfMemberDTO { Id = "1", PositionId = "1", UserId = "1" },
                new PositionWorkOfMemberDTO { Id = "2", PositionId = "2", UserId = "2" },
                new PositionWorkOfMemberDTO { Id = "3", PositionId = "3", UserId = "3" },
                new PositionWorkOfMemberDTO { Id = "4", PositionId = "4", UserId = "4" },
                new PositionWorkOfMemberDTO { Id = "5", PositionId = "5", UserId = "5" }
            );
        }
        #endregion
    }
    #endregion
}
