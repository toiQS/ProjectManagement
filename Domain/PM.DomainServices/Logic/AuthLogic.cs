using PM.DomainServices.IServices;
using PM.DomainServices.Models.auths;
using PM.DomainServices.Models;
using PM.Persistence.IServices;

namespace PM.DomainServices.Logic
{
    public class AuthLogic
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
            if (!login.Status || login.Data == null) return ServicesResult<AppUserClaim>.Failure(login.Message);
            else
            {
                var getRole = await _applicationUserServices.GetRoleOfUserByEmail(loginModel.Email);
                if (!getRole.Status) return ServicesResult<AppUserClaim>.Failure(getRole.Message);
                
                var data = new AppUserClaim()
                {
                    Email = loginModel.Email,
                    RoleUser = getRole.Data ?? "Empty"
                };
                return ServicesResult<AppUserClaim>.Success(data, string.Empty);
            }
        }
        public async Task<ServicesResult<bool>> Register(RegisterModel registerModel)
        {
            if (registerModel == null) return ServicesResult<bool>.Failure("");
            var register = await _authServices.Register(registerModel.Email, registerModel.UserName, registerModel.Password);
            if (!register.Status) return ServicesResult<bool>.Failure(register.Message);
            return ServicesResult<bool>.Success(true, string.Empty);
        }
    }
}

