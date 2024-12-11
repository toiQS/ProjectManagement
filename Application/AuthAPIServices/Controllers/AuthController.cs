using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.DomainServices.ILogic;
using PM.Infrastructure.Jwts;
using PM.Infrastructure.Loggers;
using Shared;
using Shared.appUser;

namespace AuthAPIServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthLogic _authLogic;
        private readonly ILoggerHelper<AuthController> _loggerHelper;
        private readonly IJwtHelper _jwtHelper;
        public Task<IActionResult> Login(LoginModel loginModel)
        {

        }
        public async Task<IActionResult> Logout(string token)
        {
            try
            {
                if (token == null)
                {
                    _loggerHelper.LogWarning("");
                    return BadRequest();
                }
                //var 
            }
            catch (Exception ex)
            {

            }
        }
        public async Task<IActionResult> Register(RegiserModel regiserModel)
        {
            try
            {
                var register = await _authLogic.Register(regiserModel);
                if (register.Status)
                {
                    _loggerHelper.LogInfo("");
                    return Ok();
                }
                else
                {
                    _loggerHelper.LogInfo("");
                    return BadRequest(register);
                }
                
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex,"");
                throw;
            }
        }
        public Task<IActionResult> Detail(string userId)
        {

        }
    }
}
