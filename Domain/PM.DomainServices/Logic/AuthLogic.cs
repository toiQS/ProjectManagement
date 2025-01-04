using PM.DomainServices.Models.auths;
using PM.DomainServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PM.DomainServices.IServices;
using PM.DomainServices.ILogic;
using PM.Persistence.IServices;
using Microsoft.AspNetCore.Http.HttpResults;

namespace PM.DomainServices.Logic
{
    public class AuthLogic : IAuthLogic
    {
        private readonly IAuthServices _authServices;
        private readonly IApplicationUserServices _applicationUserServices;
        public AuthLogic(IAuthServices authServices, IApplicationUserServices applicationUserServices)
        {
            _authServices = authServices;
            _applicationUserServices = applicationUserServices;
        }

        public async Task<ServicesResult<AppUserClaim>> Login(LoginModel loginModel)
        {
            if (loginModel == null) return ServicesResult<AppUserClaim>.Failure("");
            var login = await _authServices.Login(loginModel.Email, loginModel.Password);
            if (!login.Status) return ServicesResult<AppUserClaim>.Failure("");
            else
            {
                var getRole = await _applicationUserServices.GetRoleOfUserByEmail(loginModel.Email);
                if (!getRole.Status) return ServicesResult < AppUserClaim >.Failure("");
                if (login.Data == null) return ServicesResult<AppUserClaim>.Success(null);
                var data = new AppUserClaim()
                {
                    Email = loginModel.Email,
                    RoleUser = getRole.Data ?? "Empty"
                };
                return ServicesResult<AppUserClaim>.Success(data);
            }
        }
        public async Task<ServicesResult<bool>> Register(RegisterModel registerModel)
        {
            if (registerModel == null) return ServicesResult<bool>.Failure("");
            var register = await _authServices.Register(registerModel.Email, registerModel.UserName, registerModel.Password);
            if (!register.Status) return ServicesResult<bool>.Failure("");
            return ServicesResult<bool>.Success(true);
        }
    }
}
