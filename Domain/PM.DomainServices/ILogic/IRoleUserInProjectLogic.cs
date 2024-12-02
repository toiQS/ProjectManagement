using PM.Domain;
using PM.DomainServices.Shared;

namespace PM.DomainServices.ILogic
{
    public interface IRoleUserInProjectLogic
    {
        public Task<ServicesResult<bool>> Add(string userId,string projectId, string applicationUserId, string roleInProjectId);
        public Task<ServicesResult<bool>> Update(string userId, string roleUserInProjectId, string applicationUserId, string roleInProjectId);
        public Task<ServicesResult<bool>> Delete(string userId, string roleUserInProjectId);
        public Task<ServicesResult<bool>> GetRoleUserInProject(string projectId);
    }
}