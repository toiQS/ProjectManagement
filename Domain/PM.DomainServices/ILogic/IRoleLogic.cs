using PM.Domain;
using PM.DomainServices.Models;

namespace PM.DomainServices.ILogic
{
    public interface IRoleLogic
    {
        public Task<ServicesResult<RoleInProject>> GetOwnerRole();
        public Task<ServicesResult<RoleInProject>> GetLeaderRole();
        public Task<ServicesResult<RoleInProject>> GetManagerRole();
        public Task<ServicesResult<RoleInProject>> GetInfoRole(string roleId);
    }
}
