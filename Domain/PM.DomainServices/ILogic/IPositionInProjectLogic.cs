using Microsoft.Identity.Client;
using PM.Domain;
using PM.DomainServices.Shared;

namespace PM.DomainServices.ILogic
{
    public interface IPositionInProjectLogic
    {
        public Task<ServicesResult<bool>> Add(string userId, string name, string description, string projectId);
        public Task<ServicesResult<bool>> Update(string userId, string positionInProjectId, string name, string description);
        public Task<ServicesResult<bool>> Delete(string userId, string positionInProjectId);
        public Task<ServicesResult<IEnumerable<PositionInProject>>> GetPositionsInProjectByProjectId(string projectId);
        public Task<ServicesResult<PositionInProject>> GetPositionInProjectByPositionId(string positionId);
    }
}