using Microsoft.AspNetCore.Mvc;
using PM.DomainServices.ILogic;
using PM.DomainServices.Logic;
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
            if (!ModelState.IsValid)
                return BadRequest(new { Error = "Invalid model state.", Details = ModelState });

            try
            {
                // Attempt to log in the user
                var loginResult = await _auth.Login(model);
                if (!loginResult.Status)
                    return Unauthorized(new { Error = loginResult.Message });

                // Retrieve user role
                var roleResult = await _auth.GetRoleByEmail(model.Email);
                if (roleResult.Data == null)
                    return BadRequest(new { Error = roleResult.Message });

                // Generate JWT token
                var token = _jwtHelper.GenerateToken(model.Email, roleResult.Data);
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
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Errors = ModelState });

            try
            {
                // Attempt to register the user
                var registerResult = await _auth.RegisterUser(model);
                if (registerResult.Status)
                    return Ok(new { Message = "User registered successfully." });

                return BadRequest(new { Error = registerResult.Message });
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error occurred during registration process.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An internal error occurred." });
            }
        }

    }
}
