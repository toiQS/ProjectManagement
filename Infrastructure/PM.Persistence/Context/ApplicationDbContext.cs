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
        public DbSet<PlanDTO> PlanDTO { get; set; }
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

           
            
        }

    }
    #endregion
}
