using Microsoft.AspNetCore.Mvc;
using PM.DomainServices.ILogic;
using PM.Infrastructure.Jwts;
using PM.Infrastructure.Loggers;
using Shared.appUser;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthLogic _auth;
        private readonly IJwtHelper _jwtHelper;
        private readonly ILoggerHelper<AuthController> _loggerHelper;
        public AuthController(IAuthLogic auth, IJwtHelper jwtHelper, ILoggerHelper<AuthController> loggerHelper)
        {
            _auth = auth;
            _jwtHelper = jwtHelper;
            _loggerHelper = loggerHelper;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var response = await _auth.Login(model);
            if (!response.Status)
                return BadRequest(response.Message);
            var roleUser = await _auth.GetRoleByEmail(model.Email);
            var token = _jwtHelper.GenerateToken(model.Email,roleUser.Data);
            return Ok(token);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var response = await _auth.RegisterUser(model);
            if (!response.Status)
                return BadRequest(response.Message);
            return Ok("Success");
        }

    }
}
