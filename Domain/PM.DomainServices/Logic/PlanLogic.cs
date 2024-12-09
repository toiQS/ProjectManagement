using PM.Persistence.IServices;
using Shared;
using Shared.plan;

namespace PM.DomainServices.Logic
{
    public class PlanLogic
    {
        private readonly IApplicationUserServices _applicationUserServices;
        private readonly ITaskInPlanServices _taskInPlanServices;
        private readonly ITaskServices _taskServices;
        private readonly IMemberInTaskServices _memberInTaskServices;
        private readonly IPositionInProjectServices _positionInProjectServices;
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserInProjectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        private readonly IPositionWorkOfMemberServices _positionWorkOfMemberServices;
        private readonly IPlanServices _planServices;
        private readonly IProjectServices _projectServices;
        private readonly IPlanInProjectServices _planInProjectServices;
        private readonly IStatusServices _statusServices;


        public Task<ServicesResult<IEnumerable<IndexPlan>>> GetPlansInProjectId(string userId, string projectId);
        public Task<ServicesResult<DetailPlan>> GetDetailPlanById(string userId, string planId);
        public Task<ServicesResult<bool>> Add(string userId, string projectId, AddPlan addPlan);
        public Task<ServicesResult<bool>> Upd(string userId, string planId, UpdatePlan updatePlan);
        public Task<ServicesResult<bool>> Delete(string userId, string planId);
    }
}
