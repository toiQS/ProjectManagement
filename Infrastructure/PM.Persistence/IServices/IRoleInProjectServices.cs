using PM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IRoleInProjectServices
    {
        public Task<bool> AddAsync(RoleInProject roleInProjectDTO);
        public Task<bool> RemoveAsync(string Id);
        public Task<bool> UpdateAsync(string Id, RoleInProject roleInProjectDTO);
        public Task<string> GetNameRoleByRoleId(string roleId);
        public Task<RoleInProject> GetRoleInProjectByRoleId(string roleId);
        public Task<IEnumerable<RoleInProject>> GetAllAsync();
    }
}
