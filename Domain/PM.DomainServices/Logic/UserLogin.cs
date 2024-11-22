using PM.Persistence.IServices;
using PM.DomainServices.Shared;

namespace PM.DomainServices.Logic
{
    public class UserLogic
    {
        private readonly IApplicationUserServices _userServices;
        public UserLogic(IApplicationUserServices applicationUserServices)
        {
            _userServices = applicationUserServices;
        }
        public async Task<ServicesResult<bool>> Login(string email, string password )
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return ServicesResult<bool>.Failure("");
            }
            if(await _userServices.LoginServices(email, password)) return ServicesResult<bool>.Success(true);
            return ServicesResult<bool>.Failure("Login Failed");
        }
        public async Task<ServicesResult<bool>> RegisterUser(string userName ,string email, string password)
        {
            if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return ServicesResult<bool>.Failure("");
            }
            if(await _userServices.RegisterApplicationUser(userName, email,password))
            {
                return ServicesResult<bool>.Success(true);
            }
            return ServicesResult<bool>.Failure("");
        }
    }
}