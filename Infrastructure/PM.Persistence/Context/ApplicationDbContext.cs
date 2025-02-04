using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PM.Domain;

namespace PM.Persistence.Context
{
    #region register entities and seeding data
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        #region register entities
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<MemberInTask> MemberInTask { get; set; }
        public DbSet<Plan> Plan { get; set; }
        public DbSet<PlanInProject> PlanInProject { get; set; }
        public DbSet<PositionInProject> PositionInProject { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<MemberProject> MemberProject { get; set; }
        public DbSet<RoleInProject> RoleInProject { get; set; }
        public DbSet<TaskDTO> TaskDTO { get; set; }
        public DbSet<TaskInPlan> TaskInPlan { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
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

            modelBuilder.Entity<PlanInProject>()
                .HasOne(pip => pip.Plan)
                .WithMany()
                .HasForeignKey(pip => pip.PlanId)
                .OnDelete(DeleteBehavior.NoAction); // Chặn cascade delete

            modelBuilder.Entity<PlanInProject>()
                .HasOne(pip => pip.Project)
                .WithMany()
                .HasForeignKey(pip => pip.ProjectId)
                .OnDelete(DeleteBehavior.NoAction); // Chặn cascade delete

            modelBuilder.Entity<TaskInPlan>()
                .HasOne(tip => tip.TaskDTO)
                .WithMany()
                .HasForeignKey(tip => tip.TaskId)
                .OnDelete(DeleteBehavior.NoAction); // Ngăn chặn cascade delete

            modelBuilder.Entity<TaskInPlan>()
                .HasOne(tip => tip.Plan)
                .WithMany()
                .HasForeignKey(tip => tip.PlanId)
                .OnDelete(DeleteBehavior.NoAction); // Ngăn chặn cascade delete

            modelBuilder.Entity<MemberInTask>()
                .HasOne(mit => mit.TaskDTO)
                .WithMany()
                .HasForeignKey(mit => mit.TaskId)
                .OnDelete(DeleteBehavior.NoAction); // Ngăn chặn cascade delete
        }

    }
    #endregion
}
