using PM.Domain;
using PM.DomainServices.Repository;
using PM.Persistence.Context;

namespace PM.DomainServices.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IRepository<ApplicationUser> _applicationUser;
        private IRepository<MemberInTask> _memberInTask;
        private IRepository<MemberProject> _memberProject;
        private IRepository<Plan> _plan;
        private IRepository<PositionInProject> _positionInProject;
        private IRepository<Project> _project;
        private IRepository<RefreshToken> _refreshToken;
        private IRepository<RoleInProject> _roleInProject;
        private IRepository<Status> _status;
        private IRepository<TaskDTO> _taskDTO;
        private IRepository<TaskInPlan> _taskInPlan;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRepository<ApplicationUser> ApplicationUserRepository =>
            _applicationUser ??= new Repository<ApplicationUser>(_context);
        public IRepository<MemberInTask> MemberInTaskRepository =>
            _memberInTask ??= new Repository<MemberInTask>(_context);
        public IRepository<MemberProject> MemberProjectRepository =>
            _memberProject ??= new Repository<MemberProject>(_context);
        public IRepository<Plan> PlanRepository =>
            _plan ??= new Repository<Plan>(_context);
        public IRepository<PositionInProject> PositionInProjectRepository =>
            _positionInProject ??= new Repository<PositionInProject>(_context);
        public IRepository<Project> ProjectRepository =>
            _project ??= new Repository<Project>(_context);
        public IRepository<RefreshToken> RefreshTokenRepository =>
            _refreshToken ??= new Repository<RefreshToken>(_context);
        public IRepository<RoleInProject> RoleInProjectRepository =>
            _roleInProject ??= new Repository<RoleInProject>(_context);
        public IRepository<Status> StatusRepository =>
            _status ??= new Repository<Status>(_context);
        public IRepository<TaskDTO> TaskDTORepository =>
            _taskDTO ??= new Repository<TaskDTO>(_context);
        public IRepository<TaskInPlan> TaskInPlanRepository =>
            _taskInPlan ??= new Repository<TaskInPlan>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
