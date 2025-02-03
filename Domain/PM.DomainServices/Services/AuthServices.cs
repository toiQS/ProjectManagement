using PM.DomainServices.ILogics;
using PM.DomainServices.Models;
using PM.DomainServices.Models.auths;
using PM.Infrastructure.jwt;

namespace PM.DomainServices.Services
{
    public class AuthServices
    {
        private readonly IAuthLogic _auth;
        private readonly IJwtHelper _jwtHelper;
        public async Task<ServicesResult<string>> Login(LoginModel loginModel)
        {
            if (loginModel == null) return ServicesResult<string>.Failure("email or password is invalid");
            var login = await _auth.LoginAsync(loginModel);
            if(login.Status == false) return ServicesResult<string>.Failure(login.Message);
            var tokne = _jwtHelper.GenerateTokenString(loginModel.Email, "gaf0");
            return ServicesResult<string>.Success(tokne,string.Empty);
        }
    }
}
