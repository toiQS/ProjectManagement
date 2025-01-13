using PM.DomainServices.Models;
using PM.DomainServices.Models.plans;

namespace PM.DomainServices.ILogic
{
    public interface IPlanLogic
    {
        Task<ServicesResult<IEnumerable<IndexPlan>>> GetPlansInProject(string projectId);
    }
}
