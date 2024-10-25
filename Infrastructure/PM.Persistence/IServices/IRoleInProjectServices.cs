using PM.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IRoleInProjectServices
    {
        public Task<bool> AddAsync(RoleInProjectDTO roleInProjectDTO);
        public Task<bool> RemoveAsync(string Id);
        public Task<bool> UpdateAsync(string Id, RoleInProjectDTO roleInProjectDTO);
        public Task<string> GetNameRoleByRoleId(string roleId);
    }
}
