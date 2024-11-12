using Microsoft.AspNetCore.Identity;
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
        public Task<bool> RegisterApplicationUser(ApplicationUser user, string password);
        public Task<bool> RegisterApplicationAdmin(ApplicationUser admin, string password);
        public Task<ApplicationUser> GetApplicationUserAsync(string id);
        public Task<IdentityRole<string>> GetRoleAsync(string roleId);
        public Task<IEnumerable<ApplicationUser>> GetAllUser();
    }
}
