using PM.Persistence.IServices;

namespace PM.DomainServices.Logic
{
    public class InitializeLogic
    {
        private readonly IMemberInTaskServices _memberInTaskServices;
        private readonly IPlanInProjectServices _planInProjectServices;
        private readonly IPlanServices _planServices; 
        private readonly IPositionInProjectServices _positionInProjectServices;
        private readonly IPositionWorkOfMemberServices _positionWorkOfMemberServices;
        private readonly IProjectServices _projectServices;
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserInProjectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        private readonly IStatusServices _statusServices;
        private readonly ITaskInPlanServices _taskInPlanServices;
        private readonly ITaskServices _taskServices;


    }
}
