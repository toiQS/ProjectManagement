using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.DomainServices.Logic;
using PM.DomainServices.Models;
using PM.DomainServices.Models.auths;
using PM.Infrastructure.jwt;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtHelper _jwtHelper;
        private readonly AuthLogic _authLogic;
        public AuthController(IJwtHelper jwtHelper, AuthLogic authLogic)
        {
            _jwtHelper = jwtHelper;
            _authLogic = authLogic;
        }
        [HttpPost]
        public async Task<ServicesResult<string>> Login(LoginModel loginModel)
        {
            var login = await _authLogic.Login(loginModel);
            if(login.Status == false) return ServicesResult<string>.Failure(login.Message);
        }
    }
}
