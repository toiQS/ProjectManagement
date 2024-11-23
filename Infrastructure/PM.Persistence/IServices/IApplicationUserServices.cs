using Microsoft.AspNetCore.Identity;
using PM.Domain;
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
        public Task<ApplicationUser> LoginServices(string email, string password);
        public Task<string> GetRoleApplicatonUserByUserIdAsync(string userId);
        public Task Logout();

    }
}
