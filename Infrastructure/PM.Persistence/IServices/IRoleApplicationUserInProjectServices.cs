using PM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IRoleApplicationUserInProjectServices
    {
        public Task<bool> AddAsync(RoleApplicationUserInProject roleApplicationUserInProjectDTO);
        public Task<bool> UpdateAsync(string id, RoleApplicationUserInProject roleApplicationUserInProjectDTO);
        public Task<bool> RemoveAsync(string id);
        public Task<IEnumerable<RoleApplicationUserInProject>> GetProjectsUserJoined(string userid);
        public Task<IEnumerable<RoleApplicationUserInProject>> GetAllAsync();
        public Task<RoleApplicationUserInProject> GetRoleApplicationUserInProjectById(string Id);
        public Task<IEnumerable<RoleApplicationUserInProject>> GetRoleApplicationUserInProjectsByProjectId(string ProjectId);
        public Task<RoleApplicationUserInProject> GetRoleApplicationUserInProjectByUserIdAndProjectId(string UserId, string ProjectId);  
    }
}
