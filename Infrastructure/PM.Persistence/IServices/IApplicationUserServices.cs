﻿using Microsoft.AspNetCore.Identity;
using PM.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IApplicationUserServices
    {
        public Task<bool> RegisterApplicationUser(string userName, string email, string password);
        public Task<bool> RegisterApplicationAdmin(string userName, string email, string password);
        public Task<ApplicationUser> GetApplicationUserAsync(string id);
        public Task<IdentityRole<string>> GetRoleAsync(string roleId);
        public Task<IEnumerable<ApplicationUser>> GetAllUser();
        public Task<bool> LoginServices(string email, string password);
        public Task Logout();

    }
}
