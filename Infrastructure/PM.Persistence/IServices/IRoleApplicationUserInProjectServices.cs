﻿using PM.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IRoleApplicationUserInProjectServices
    {
        public Task<bool> AddAsync(RoleApplicationUserInProjectDTO roleApplicationUserInProjectDTO);
        public Task<bool> UpdateAsync(string id, RoleApplicationUserInProjectDTO roleApplicationUserInProjectDTO);
        public Task<bool> RemoveAsync(string id);
        public Task<IEnumerable<RoleApplicationUserInProjectDTO>> GetProjectsUserJoined(string userid);
        public Task<IEnumerable<RoleApplicationUserInProjectDTO>> GetAllAsync();
        public Task<RoleApplicationUserInProjectDTO> GetRoleApplicationUserInProjectById(string Id);
        public Task<IEnumerable<RoleApplicationUserInProjectDTO>> GetRoleApplicationUserInProjectsByProjectId(string ProjectId);
        public Task<RoleApplicationUserInProjectDTO> GetRoleApplicationUserInProjectByUserIdAndProjectId(string UserId, string ProjectId);  
    }
}
