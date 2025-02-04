using PM.DomainServices.Models;
using PM.DomainServices.Models.auths;

namespace PM.DomainServices.ILogics
{
    public interface IAuthLogic : IDisposable
    {
        public Task<ServicesResult<LoginModel>> LoginAsync(LoginModel loginModel);
        public Task<ServicesResult<RegisterModel>> RegisterAsync(RegisterModel registerModel);
        public Task<ServicesResult<bool>> LogOutAsync();
    }
}
