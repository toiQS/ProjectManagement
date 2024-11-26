using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PM.Domain;

namespace PM.Persistence.Context
{
    #region register entities and seeding data
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole,string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
           
        }
        #region register entities
        public DbSet<ApplicationUser> ApplicationUser {  get; set; }
        public DbSet<MemberInTask> MemberInTask { get; set; }
        public DbSet<Plan> Plan { get; set; }
        public DbSet<PlanInProject> PlanInProject { get; set; }
        public DbSet<PositionWorkOfMember> PositionWorkOfMember { get; set; }
        public DbSet<PositionInProject> PostitionInProject { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<RoleApplicationUserInProject> RoleApplicationUserInProject { get; set; }
        public DbSet<RoleInProject> RoleInProject { get; set; }
        public DbSet<TaskDTO> TaskDTO { get; set; }
        public DbSet<TaskInPlan> TaskInPlan { get; set; }
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

            modelBuilder.Entity<PositionWorkOfMember>()
             .HasOne(p => p.RoleApplicationUserInProject)
             .WithMany()
             .HasForeignKey(p => p.RoleApplicationUserInProjectId)
             .OnDelete(DeleteBehavior.NoAction);

            #region seeding data
            // seeding application user
            modelBuilder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                FirstName = "nguyen",
                LastName = "a",
                FullName= "nguyen a",
                Email = "nguyena@gmail.com",
                Phone="0123456789",
                Id = $"1011{DateTime.Now}10",
                PathImage =""
            });
            #endregion
        }

    }
    #endregion
}
