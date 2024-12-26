using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.DomainServices.ILogic;
using PM.Infrastructure.Jwts;
using PM.Infrastructure.Loggers;
using Shared.position;

namespace ResourceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController : ControllerBase
    {
        #region Fields
        private readonly ILoggerHelper<PositionController> _loggerHelper;
        private readonly IPositionLogic _positionLogic;
        private readonly IJwtHelper _jwtHelper;
        private readonly IAuthLogic _authLogic;
        #endregion

        #region Constructor
        public PositionController(
            ILoggerHelper<PositionController> loggerHelper,
            IPositionLogic positionLogic,
            IJwtHelper jwtHelper,
            IAuthLogic authLogic)
        {
            _loggerHelper = loggerHelper;
            _positionLogic = positionLogic;
            _jwtHelper = jwtHelper;
            _authLogic = authLogic;
        }
        #endregion

        #region Get Methods

        /// <summary>
        /// Retrieves position details by project ID.
        /// </summary>
        /// <param name="token">JWT token for authentication.</param>
        /// <param name="projectId">ID of the project.</param>
        /// <returns>Position details.</returns>
        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetPosition(string token, string projectId)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(projectId))
                {
                    _loggerHelper.LogWarning("Token or Project ID is null or empty.");
                    return BadRequest("Token and Project ID are required.");
                }

                var result = _jwtHelper.ParseToken(token);
                if (result == null)
                {
                    _loggerHelper.LogWarning("Failed to parse token.");
                    return BadRequest("Invalid token.");
                }

                if (result.Role == "Admin")
                {
                    _loggerHelper.LogWarning("Admin role not allowed.");
                    return BadRequest("Access denied.");
                }

                var user = await _authLogic.GetUserDetailByMail(result.Email);
                if (!user.Status)
                {
                    _loggerHelper.LogDebug(user.Message);
                    return BadRequest(user);
                }

                var project = await _positionLogic.Get(user.Data.UserId, projectId);
                if (!project.Status)
                {
                    _loggerHelper.LogDebug(project.Message);
                    return BadRequest(project);
                }

                return Ok(project);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error fetching position details.");
                throw;
            }
        }

        #endregion

        #region Create Methods

        /// <summary>
        /// Adds a new position to a project.
        /// </summary>
        /// <param name="token">JWT token for authentication.</param>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="addPosition">Details of the position to be added.</param>
        /// <returns>Status of the operation.</returns>
        [HttpPost("{projectId}")]
        public async Task<IActionResult> AddNewPosition(string token, string projectId, AddPosition addPosition)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(projectId) || addPosition == null || !ModelState.IsValid)
                {
                    _loggerHelper.LogWarning("Invalid input parameters.");
                    return BadRequest("Token, Project ID, and Position details are required.");
                }

                var result = _jwtHelper.ParseToken(token);
                if (result == null)
                {
                    _loggerHelper.LogWarning("Failed to parse token.");
                    return BadRequest("Invalid token.");
                }

                if (result.Role == "Admin")
                {
                    _loggerHelper.LogWarning("Admin role not allowed.");
                    return BadRequest("Access denied.");
                }

                var user = await _authLogic.GetUserDetailByMail(result.Email);
                if (!user.Status)
                {
                    _loggerHelper.LogDebug(user.Message);
                    return BadRequest(user);
                }

                var project = await _positionLogic.Add(user.Data.UserId, projectId, addPosition);
                if (!project.Status)
                {
                    _loggerHelper.LogDebug(project.Message);
                    return BadRequest(project);
                }

                return Ok(project);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error adding new position.");
                throw;
            }
        }

        #endregion

        #region Update Methods

        /// <summary>
        /// Updates an existing position.
        /// </summary>
        /// <param name="token">JWT token for authentication.</param>
        /// <param name="positionId">ID of the position to be updated.</param>
        /// <param name="updatePosition">Updated details of the position.</param>
        /// <returns>Status of the operation.</returns>
        [HttpPut("{positionId}")]
        public async Task<IActionResult> Update(string token, string positionId, UpdatePositon updatePosition)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(positionId) || updatePosition == null || !ModelState.IsValid)
                {
                    _loggerHelper.LogWarning("Invalid input parameters.");
                    return BadRequest("Token, Position ID, and Position details are required.");
                }

                var result = _jwtHelper.ParseToken(token);
                if (result == null)
                {
                    _loggerHelper.LogWarning("Failed to parse token.");
                    return BadRequest("Invalid token.");
                }

                if (result.Role == "Admin")
                {
                    _loggerHelper.LogWarning("Admin role not allowed.");
                    return BadRequest("Access denied.");
                }

                var user = await _authLogic.GetUserDetailByMail(result.Email);
                if (!user.Status)
                {
                    _loggerHelper.LogDebug(user.Message);
                    return BadRequest(user);
                }

                var project = await _positionLogic.Update(user.Data.UserId, positionId, updatePosition);
                if (!project.Status)
                {
                    _loggerHelper.LogDebug(project.Message);
                    return BadRequest(project);
                }

                return Ok(project);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error updating position.");
                throw;
            }
        }

        #endregion

        #region Delete Methods

        /// <summary>
        /// Deletes a position from a project.
        /// </summary>
        /// <param name="token">JWT token for authentication.</param>
        /// <param name="positionId">ID of the position to be deleted.</param>
        /// <param name="projectId">ID of the project containing the position.</param>
        /// <returns>Status of the operation.</returns>
        [HttpDelete("{positionId}/{projectId}")]
        public async Task<IActionResult> Delete(string token, string positionId, string projectId)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(positionId) || string.IsNullOrEmpty(projectId))
                {
                    _loggerHelper.LogWarning("Invalid input parameters.");
                    return BadRequest("Token, Position ID, and Project ID are required.");
                }

                var result = _jwtHelper.ParseToken(token);
                if (result == null)
                {
                    _loggerHelper.LogWarning("Failed to parse token.");
                    return BadRequest("Invalid token.");
                }

                if (result.Role == "Admin")
                {
                    _loggerHelper.LogWarning("Admin role not allowed.");
                    return BadRequest("Access denied.");
                }

                var user = await _authLogic.GetUserDetailByMail(result.Email);
                if (!user.Status)
                {
                    _loggerHelper.LogDebug(user.Message);
                    return BadRequest(user);
                }

                var project = await _positionLogic.Delete(user.Data.UserId, positionId, projectId);
                if (!project.Status)
                {
                    _loggerHelper.LogDebug(project.Message);
                    return BadRequest(project);
                }

                return Ok(project);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error deleting position.");
                throw;
            }
        }

        #endregion
    }
}
