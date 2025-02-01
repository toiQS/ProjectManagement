using PM.Domain;
using PM.DomainServices.Repository;

namespace PM.DomainServices.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<ApplicationUser> ApplicationUserRepository { get; }
        IRepository<MemberInTask> MemberInTaskRepository { get; }
        IRepository<MemberProject> MemberProjectRepository { get; }
        IRepository<Plan> PlanRepository { get; }
        IRepository<PositionInProject> PositionInProjectRepository { get; }
        IRepository<Project> ProjectRepository { get; }
        IRepository<RefreshToken> RefreshTokenRepository { get; }
        IRepository<RoleInProject> RoleInProjectRepository { get; }
        IRepository<Status> StatusRepository { get; }
        IRepository<TaskDTO> TaskDTORepository { get; }
        IRepository<TaskInPlan> TaskInPlanRepository { get; }
        Task<int> SaveChangesAsync();

    }
}
