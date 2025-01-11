using PM.Domain;
using PM.DomainServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.DomainServices.Logic
{
    public interface IRoleLogic
    {
        public Task<ServicesResult<RoleInProject>> GetOwnerRole();
        public Task<ServicesResult<RoleInProject>> GetLeaderRole();
        public Task<ServicesResult<RoleInProject>> GetManagerRole();
        public Task<ServicesResult<RoleInProject>> GetInfoRole(string roleId);
    }
}
