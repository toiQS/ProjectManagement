using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.DomainServices.ILogic;
using PM.Infrastructure.Jwts;
using PM.Infrastructure.Loggers;
using Shared;
using Shared.appUser;

namespace AuthAPIService2s.Controllers
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

        #region Login
        /// <summary>
        /// Handles user login.
        /// </summary>
        /// <param name="loginModel">The login model containing email and password.</param>
        /// <returns>A token if login is successful, otherwise an error response.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Error = "Invalid model state.", Details = ModelState });

            try
            {
                // Attempt to log in the user
                var loginResult = await _authLogic.Login(loginModel);
                if (!loginResult.Status)
                    return Unauthorized(new { Error = "Invalid email or password." });

                // Retrieve user role
                var roleResult = await _authLogic.GetRoleByEmail(loginModel.Email);
                if (roleResult.Data == null)
                    return BadRequest(new { Error = "Unable to determine user role." });

                // Generate JWT token
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
        #endregion

        #region Register
        /// <summary>
        /// Handles user registration.
        /// </summary>
        /// <param name="registerModel">The registration model containing user details.</param>
        /// <returns>Status of the registration process.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Errors = ModelState });

            try
            {
                // Attempt to register the user
                var registerResult = await _authLogic.RegisterUser(registerModel);
                if (registerResult.Status)
                    return Ok(new { Message = "User registered successfully." });

                return BadRequest(new { Error = "Registration failed." });
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error occurred during registration process.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An internal error occurred." });
            }
        }
        #endregion

        #region User Details
        /// <summary>
        /// Retrieves user details using a token.
        /// </summary>
        /// <param name="token">The token used to retrieve user details.</param>
        /// <returns>User details if the token is valid.</returns>
        [HttpGet("user-details")]
        public async Task<IActionResult> DetailUser([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest(new { Error = "Token is required." });

            try
            {
                // Parse the token
                var tokenDetails = _jwtHelper.ParseToken(token);
                if (tokenDetails == null)
                    return Unauthorized(new { Error = "Invalid token." });

                // Get user details
                var userDetails = await _authLogic.GetUserDetailByMail(tokenDetails.Email);
                if (userDetails == null)
                    return NotFound(new { Error = "User not found." });

                return Ok(userDetails);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error occurred while retrieving user details.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An internal error occurred." });
            }
        }
        #endregion
    }
}
