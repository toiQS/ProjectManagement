using Microsoft.AspNetCore.Mvc;
using PM.DomainServices.Models;
using PM.DomainServices.Models.auths;
using PM.DomainServices.Models.users;

namespace PM.DomainServices.ILogic
{
    /// <summary>
    /// Interface defining authentication and user management logic.
    /// </summary>
    public interface IAuthLogic
    {
        public Task<ServicesResult<AppUserClaim>> Login(LoginModel loginModel);
        public Task<ServicesResult<bool>> Register(RegisterModel registerModel);
    }
}
