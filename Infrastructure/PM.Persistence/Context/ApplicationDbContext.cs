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

            #region seeding data
            // Seed data for ApplicationUser
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser { Id = "US1", FirstName = "John", LastName = "Doe", FullName = "John Doe", Phone = "123456789", PathImage = "john.jpg", UserName = "john_doe", Email = "john.doe@example.com" },
                new ApplicationUser { Id = "US2", FirstName = "Jane", LastName = "Smith", FullName = "Jane Smith", Phone = "987654321", PathImage = "jane.jpg", UserName = "jane_smith", Email = "jane.smith@example.com" },
                new ApplicationUser { Id = "US3", FirstName = "Michael", LastName = "Johnson", FullName = "Michael Johnson", Phone = "234567890", PathImage = "michael.jpg", UserName = "michael_j", Email = "michael.j@example.com" },
                new ApplicationUser { Id = "US4", FirstName = "Emily", LastName = "Davis", FullName = "Emily Davis", Phone = "345678901", PathImage = "emily.jpg", UserName = "emily_d", Email = "emily.d@example.com" },
                new ApplicationUser { Id = "US5", FirstName = "Chris", LastName = "Brown", FullName = "Chris Brown", Phone = "456789012", PathImage = "chris.jpg", UserName = "chris_b", Email = "chris.b@example.com" }
            );

            // Seed data for ProjectDTO
            modelBuilder.Entity<ProjectDTO>().HasData(
                new ProjectDTO { Id = "PR1", ProjectName = "Project Alpha", ProjectDescription = "First project description", ProjectVersion = "1.0", Projectstatus = "In Progress", CreateAt = DateTime.Now.ToString(), IsDeleted = false, IsModified = true },
                new ProjectDTO { Id = "PR2", ProjectName = "Project Beta", ProjectDescription = "Second project description", ProjectVersion = "1.1", Projectstatus = "Completed", CreateAt = DateTime.Now.ToString(), IsDeleted = false, IsModified = true },
                new ProjectDTO { Id = "PR3", ProjectName = "Project Gamma", ProjectDescription = "Third project description", ProjectVersion = "1.2", Projectstatus = "On Hold", CreateAt = DateTime.Now.ToString(), IsDeleted = false , IsModified = false},
                new ProjectDTO { Id = "Pr4", ProjectName = "Project Delta", ProjectDescription = "Fourth project description", ProjectVersion = "1.3", Projectstatus = "Cancelled", CreateAt = DateTime.Now.ToString(), IsDeleted = false , IsModified = false},
                new ProjectDTO { Id = "Pr5", ProjectName = "Project Epsilon", ProjectDescription = "Fifth project description", ProjectVersion = "2.0", Projectstatus = "In Progress", CreateAt = DateTime.Now.ToString(), IsDeleted = false , IsModified = false }
            );

            // Seed data for TaskDTO
            modelBuilder.Entity<TaskDTO>().HasData(
                new TaskDTO { Id = "TA1", TaskName = "Task 1", TaskDescription = "Task description for project 1", TaskStatus = "Pending", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(1), EndAt = DateTime.Now.AddDays(5) },
                new TaskDTO { Id = "TA2", TaskName = "Task 2", TaskDescription = "Task description for project 2", TaskStatus = "Completed", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(2), EndAt = DateTime.Now.AddDays(6) },
                new TaskDTO { Id = "TA3", TaskName = "Task 3", TaskDescription = "Task description for project 3", TaskStatus = "In Progress", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(3), EndAt = DateTime.Now.AddDays(7) },
                new TaskDTO { Id = "TA4", TaskName = "Task 4", TaskDescription = "Task description for project 4", TaskStatus = "On Hold", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(4), EndAt = DateTime.Now.AddDays(8) },
                new TaskDTO { Id = "TA5", TaskName = "Task 5", TaskDescription = "Task description for project 5", TaskStatus = "Pending", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(5), EndAt = DateTime.Now.AddDays(9) }
            );

            // Seed data for PlanDTOs
            modelBuilder.Entity<PlanDTOs>().HasData(
                new PlanDTOs { Id = "PL1", PlanName = "Plan A", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(1), EndAt = DateTime.Now.AddMonths(1) },
                new PlanDTOs { Id = "PL2", PlanName = "Plan B", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(2), EndAt = DateTime.Now.AddMonths(2) },
                new PlanDTOs { Id = "PL3", PlanName = "Plan C", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(3), EndAt = DateTime.Now.AddMonths(3) },
                new PlanDTOs { Id = "PL4", PlanName = "Plan D", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(4), EndAt = DateTime.Now.AddMonths(4) },
                new PlanDTOs { Id = "PL5", PlanName = "Plan E", CreateAt = DateTime.Now, StartAt = DateTime.Now.AddDays(5), EndAt = DateTime.Now.AddMonths(5) }
            );

            // Seed data for TaskInPlanDTO
            modelBuilder.Entity<TaskInPlanDTO>().HasData(
                new TaskInPlanDTO { Id = "TIP1", PlanId = "PL1", TaskId = "TA1" },
                new TaskInPlanDTO { Id = "TIP2", PlanId = "PL2", TaskId = "TA2" },
                new TaskInPlanDTO { Id = "TIP3", PlanId = "PL3", TaskId = "TA3" },
                new TaskInPlanDTO { Id = "TIP4", PlanId = "PL4", TaskId = "TA4" },
                new TaskInPlanDTO { Id = "TIP5", PlanId = "PL5", TaskId = "TA5" }
            );

            // Seed data for MemberInTaskDTO
            modelBuilder.Entity<MemberInTaskDTO>().HasData(
                new MemberInTaskDTO { Id = "MIT1", PositionWorkOfMemberId = "PWOM1", TaskId = "TA1" },
                new MemberInTaskDTO { Id = "MIT2", PositionWorkOfMemberId = "PWOM2", TaskId = "TA2" },
                new MemberInTaskDTO { Id = "MIT3", PositionWorkOfMemberId = "PWOM3", TaskId = "TA3" },
                new MemberInTaskDTO { Id = "MIT4", PositionWorkOfMemberId = "PWOM4", TaskId = "TA4" },
                new MemberInTaskDTO { Id = "MIT5", PositionWorkOfMemberId = "PWOM5", TaskId = "TA5" }
            );

            // Seed data for PostitionInProjectDTO
            modelBuilder.Entity<PostitionInProjectDTO>().HasData(
                new PostitionInProjectDTO { Id = "POIP1", PositionName = "Manager", PositionDescription = "Project Manager", ProjectId = "PR1" },
                new PostitionInProjectDTO { Id = "POIP2", PositionName = "Developer", PositionDescription = "Software Developer", ProjectId = "PR2" },
                new PostitionInProjectDTO { Id = "POIP3", PositionName = "Tester", PositionDescription = "Software Tester", ProjectId = "PR3" },
                new PostitionInProjectDTO { Id = "POIP4", PositionName = "Designer", PositionDescription = "UI/UX Designer", ProjectId = "PR4" },
                new PostitionInProjectDTO { Id = "POIP5", PositionName = "DevOps", PositionDescription = "DevOps Engineer", ProjectId = "PR5" }
            );

            // Seed data for RoleInProjectDTO
            modelBuilder.Entity<RoleInProjectDTO>().HasData(
                new RoleInProjectDTO { Id = "RIP1", RoleName = "Admin", RoleDescription = "Administrator Role" },
                new RoleInProjectDTO { Id = "RIP2", RoleName = "Contributor", RoleDescription = "Contributor Role" },
                new RoleInProjectDTO { Id = "RIP3", RoleName = "Viewer", RoleDescription = "Viewer Role" },
                new RoleInProjectDTO { Id = "RIP4", RoleName = "Editor", RoleDescription = "Editor Role" },
                new RoleInProjectDTO { Id = "RIP5", RoleName = "Owner", RoleDescription = "Owner Role" }
            );

            // Seed data for RoleApplicationUserInProjectDTO
            modelBuilder.Entity<RoleApplicationUserInProjectDTO>().HasData(
                new RoleApplicationUserInProjectDTO { Id = "RAUIP1", ProjectId = "PR1", RoleInProjectId = "RIP1" },
                new RoleApplicationUserInProjectDTO { Id = "RAUIP2", ProjectId = "PR2", RoleInProjectId = "RIP2" },
                new RoleApplicationUserInProjectDTO { Id = "RAUIP3", ProjectId = "PR3", RoleInProjectId = "RIP3" },
                new RoleApplicationUserInProjectDTO { Id = "RAUIP4", ProjectId = "PR4", RoleInProjectId = "RIP4" },
                new RoleApplicationUserInProjectDTO { Id = "RAUIP5", ProjectId = "PR5", RoleInProjectId = "RIP5" }
            );

            // Seed data for PlanInProjectDTO
            modelBuilder.Entity<PlanInProjectDTO>().HasData(
                new PlanInProjectDTO { Id = "PLIP1", ProjectId = "PR1", PlanId = "PL1" },
                new PlanInProjectDTO { Id = "PLIP2", ProjectId = "PR2", PlanId = "PL2" },
                new PlanInProjectDTO { Id = "PLIP3", ProjectId = "PR3", PlanId = "PL3" },
                new PlanInProjectDTO { Id = "PLIP4", ProjectId = "PR4", PlanId = "PL4" },
                new PlanInProjectDTO { Id = "PLIP5", ProjectId = "PR5", PlanId = "PL5" }
            );

            // Seed data for PositionWorkOfMemberDTO
            modelBuilder.Entity<PositionWorkOfMemberDTO>().HasData(
                new PositionWorkOfMemberDTO { Id = "PWOM1", PostitionInProjectId = "POIP1", UserId = "US1" },
                new PositionWorkOfMemberDTO { Id = "PWOM2", PostitionInProjectId = "POIP2", UserId = "US2" },
                new PositionWorkOfMemberDTO { Id = "PWOM3", PostitionInProjectId = "POIP3", UserId = "US3" },
                new PositionWorkOfMemberDTO { Id = "PWOM4", PostitionInProjectId = "POIP4", UserId = "US4" },
                new PositionWorkOfMemberDTO { Id = "PWOM5", PostitionInProjectId = "POIP5", UserId = "US5" }
            );
            #endregion
        }

    }
    #endregion
}
