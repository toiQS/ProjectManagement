using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Domain;
using PM.DomainServices.ILogic;
using PM.Infrastructure.Jwts;
using PM.Infrastructure.Loggers;
using Shared.project;

namespace ProjectAPIServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        #region Fields
        private readonly IProjectLogic _projectLogic;
        private readonly IJwtHelper _jwtHelper;
        private readonly ILoggerHelper<ProjectController> _loggerHelper;
        private readonly IAuthLogic _authLogic;
        #endregion

        #region Constructor
        public ProjectController(
            IProjectLogic projectLogic,
            IJwtHelper jwtHelper,
            ILoggerHelper<ProjectController> loggerHelper,
            IAuthLogic authLogic)
        {
            _projectLogic = projectLogic;
            _jwtHelper = jwtHelper;
            _loggerHelper = loggerHelper;
            _authLogic = authLogic;
        }
        #endregion

        #region Get Methods
        /// <summary>
        /// Retrieves the list of projects a user has joined.
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>List of joined projects</returns>
        [HttpGet("joined-projects")]
        public async Task<IActionResult> GetProductListUserHasJoined(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    _loggerHelper.LogWarning("Token is null or empty.");
                    return BadRequest("Token is required.");
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

                var project = await _projectLogic.GetProductListUserHasJoined(user.Data.UserId);
                if (!project.Status)
                {
                    _loggerHelper.LogDebug(project.Message);
                    return BadRequest(project);
                }

                return Ok(project);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error fetching joined projects.");
                throw;
            }
        }

        /// <summary>
        /// Retrieves the list of projects a user owns.
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>List of owned projects</returns>
        [HttpGet("owned-projects")]
        public async Task<IActionResult> GetProductListUserHasOwner(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    _loggerHelper.LogWarning("Token is null or empty.");
                    return BadRequest("Token is required.");
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

                var project = await _projectLogic.GetProductListUserHasOwner(user.Data.UserId);
                if (!project.Status)
                {
                    _loggerHelper.LogDebug(project.Message);
                    return BadRequest(project);
                }

                return Ok(project);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error fetching owned projects.");
                throw;
            }
        }
        #endregion

        #region Project Details
        /// <summary>
        /// Retrieves the details of a project a user has joined.
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <param name="projectId">Project ID</param>
        /// <returns>Project details</returns>
        [HttpGet("joined-project-details")]
        public async Task<IActionResult> GetProductDetailProjectHasJoined(string token, string projectId)
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

                var project = await _projectLogic.GetProductDetailProjectHasJoined(user.Data.UserId, projectId);
                if (!project.Status)
                {
                    _loggerHelper.LogDebug(project.Message);
                    return BadRequest(project);
                }

                return Ok(project);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error fetching project details.");
                throw;
            }
        }
        #endregion

        #region Project Modification Methods
        /// <summary>
        /// Adds a new project.
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <param name="addProject">Project data</param>
        /// <returns>Operation result</returns>
        [HttpPost("add-project")]
        public async Task<IActionResult> AddProject(string token, AddProject addProject)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(token) || addProject == null)
            {
                _loggerHelper.LogWarning("Invalid input.");
                return BadRequest("Invalid input.");
            }

            try
            {
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

                var resultAdd = await _projectLogic.Add(user.Data.UserId, addProject);
                if (!resultAdd.Status)
                {
                    _loggerHelper.LogWarning(resultAdd.Message);
                    return BadRequest(resultAdd);
                }

                return Ok(resultAdd);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error adding project.");
                throw;
            }
        }
        #endregion
        /// <summary>
        /// Updates a project's information.
        /// </summary>
        [HttpPut("update-project")]
        public async Task<IActionResult> Update(string token, string projectId, UpdateProject updateProject)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(projectId) || updateProject == null)
            {
                _loggerHelper.LogWarning("Invalid input.");
                return BadRequest("Invalid input.");
            }

            try
            {
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
                    return BadRequest();
                }

                var resultAdd = await _projectLogic.UpdateInfo(user.Data.UserId, projectId, updateProject);
                if (!resultAdd.Status)
                {
                    _loggerHelper.LogWarning(resultAdd.Message);
                    return BadRequest(resultAdd);
                }

                return Ok(resultAdd);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error updating project.");
                throw;
            }
        }

        /// <summary>
        /// Deletes a project.
        /// </summary>
        [HttpDelete("delete-project")]
        public async Task<IActionResult> Delete(string token, string projectId)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(token) || projectId == null)
            {
                _loggerHelper.LogWarning("Invalid input.");
                return BadRequest("Invalid input.");
            }

            try
            {
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
                    return BadRequest();
                }

                var resultAdd = await _projectLogic.Delete(user.Data.UserId, projectId);
                if (!resultAdd.Status)
                {
                    _loggerHelper.LogWarning(resultAdd.Message);
                    return BadRequest(resultAdd);
                }

                return Ok(resultAdd);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error deleting project.");
                throw;
            }
        }

        /// <summary>
        /// Updates the IsDelete flag of a project.
        /// </summary>
        [HttpPatch("update-is-delete")]
        public async Task<IActionResult> UpdateIsDelete(string token, string projectId)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(token) || projectId == null)
            {
                _loggerHelper.LogWarning("Invalid input.");
                return BadRequest("Invalid input.");
            }

            try
            {
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
                    return BadRequest();
                }

                var resultAdd = await _projectLogic.UpdateIsDelete(user.Data.UserId, projectId);
                if (!resultAdd.Status)
                {
                    _loggerHelper.LogWarning(resultAdd.Message);
                    return BadRequest(resultAdd);
                }

                return Ok(resultAdd);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error updating IsDelete flag.");
                throw;
            }
        }

        /// <summary>
        /// Updates the IsDone flag of a project.
        /// </summary>
        [HttpPatch("update-is-done")]
        public async Task<IActionResult> UpdateIsDone(string token, string projectId)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(token) || projectId == null)
            {
                _loggerHelper.LogWarning("Invalid input.");
                return BadRequest("Invalid input.");
            }

            try
            {
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
                    return BadRequest();
                }

                var resultAdd = await _projectLogic.UpdateIsDone(user.Data.UserId, projectId);
                if (!resultAdd.Status)
                {
                    _loggerHelper.LogWarning(resultAdd.Message);
                    return BadRequest(resultAdd);
                }

                return Ok(resultAdd);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error updating IsDone flag.");
                throw;
            }
        }

        /// <summary>
        /// Updates the status of a project.
        /// </summary>
        [HttpPatch("update-status")]
        public async Task<IActionResult> UpdateStatus(string token, string projectId)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(token) || projectId == null)
            {
                _loggerHelper.LogWarning("Invalid input.");
                return BadRequest("Invalid input.");
            }

            try
            {
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
                    return BadRequest();
                }

                var resultAdd = await _projectLogic.UpdateStatus(user.Data.UserId, projectId);
                if (!resultAdd.Status)
                {
                    _loggerHelper.LogWarning(resultAdd.Message);
                    return BadRequest(resultAdd);
                }

                return Ok(resultAdd);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
