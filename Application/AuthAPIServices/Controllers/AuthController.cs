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

        public AuthController(IAuthLogic authLogic, ILoggerHelper<AuthController> loggerHelper, IJwtHelper jwtHelper)
        {
            _authLogic = authLogic;
            _loggerHelper = loggerHelper;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Error = "Invalid model state.", Details = ModelState });

            try
            {
                var loginResult = await _authLogic.Login(loginModel);
                if (!loginResult.Status)
                    return Unauthorized(new { Error = "Invalid email or password." });

                var roleResult = await _authLogic.GetRoleByEmail(loginModel.Email);
                if (roleResult.Data == null)
                    return BadRequest(new { Error = "Unable to determine user role." });

                var token = _jwtHelper.GenerateToken(loginModel.Email, roleResult.Data);
                if (string.IsNullOrEmpty(token))
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Token generation failed." });

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error occurred during login process.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An internal error occurred." });
            }
        }

    }
}
