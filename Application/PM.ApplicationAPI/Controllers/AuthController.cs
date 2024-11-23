using Microsoft.AspNetCore.Mvc;
using PM.DomainServices.ILogic;
using PM.Infrastructure.Jwts;
using PM.Infrastructure.Loggers;

namespace PM.ApplicationAPI.Controllers
{
    /// <summary>
    /// Controller for handling authentication operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILoggerHelper<AuthController> _loggerHelper;
        private readonly IJwtHelper _jwtHelper;
        private readonly IUserLogic _userLogic;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="userLogic">The user logic service for handling user operations.</param>
        /// <param name="jwtHelper">The JWT helper service for generating tokens.</param>
        /// <param name="loggerHelper">The logger service for logging information.</param>
        public AuthController(IUserLogic userLogic, IJwtHelper jwtHelper, ILoggerHelper<AuthController> loggerHelper)
        {
            _userLogic = userLogic;
            _jwtHelper = jwtHelper;
            _loggerHelper = loggerHelper;
        }

        /// <summary>
        /// Handles user login and generates a JWT token if the login is successful.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A JWT token if the login is successful, or an error message if it fails.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> LoginTask(string email, string password)
        {
            // Attempt to log in the user
            var login = await _userLogic.Login(email, password);

            if (login.Status)
            {
                // Retrieve the user ID and role
                var userId = login.Data.Id;
                var roleResult = await _userLogic.GetRoleNameOfUser(userId);

                if (!roleResult.Status)
                {
                    // Return role retrieval failure message
                    return BadRequest(roleResult.Message);
                }

                var roleName = roleResult.Data;

                // Generate a JWT token with the user ID and role
                var token = _jwtHelper.GenerateJwtToken(userId, roleName);
                return Ok(new { Token = token });
            }
            else
            {
                // Log the login failure and return the failure message
                var message = login.Message;
                _loggerHelper.LogInfo(message);
                return Unauthorized(new { Error = message });
            }
        }
        [HttpPost(Name ="register user")]
        public async Task<IActionResult> RegisterUserTask(string userName, string email, string password)
        {
            var register = await _userLogic.RegisterUser(userName, email, password);
            if (register.Status)
            {
                return Ok();
            }
            else
            {
                var message = register.Message;
                return BadRequest($"{message}");
            }
        }
        
    }
}
