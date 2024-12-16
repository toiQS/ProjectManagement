using Microsoft.AspNetCore.Mvc;
using PM.DomainServices.ILogic;
using PM.Infrastructure.Jwts;
using PM.Infrastructure.Loggers;
using Shared.plan;

namespace PlanAPIServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanController : ControllerBase
    {
        #region Fields
        private readonly ILoggerHelper<PlanController> _loggerHelper;
        private readonly IJwtHelper _jwtHelper;
        private readonly IPlanLogic _planLogic;
        private readonly IAuthLogic _authLogic;
        #endregion

        #region Constructor
        public PlanController(
            ILoggerHelper<PlanController> loggerHelper,
            IJwtHelper jwtHelper,
            IPlanLogic planLogic,
            IAuthLogic authLogic)
        {
            _loggerHelper = loggerHelper;
            _jwtHelper = jwtHelper;
            _planLogic = planLogic;
            _authLogic = authLogic;
        }
        #endregion

        #region Get Methods

        /// <summary>
        /// Retrieves all plans associated with a specific project ID.
        /// </summary>
        /// <param name="token">JWT token for authentication.</param>
        /// <param name="projectId">ID of the project.</param>
        /// <returns>List of plans in the project.</returns>
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetPlansInProjectId(string token, string projectId)
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

                var plans = await _planLogic.GetPlansInProjectId(user.Data.UserId, projectId);
                if (!plans.Status)
                {
                    _loggerHelper.LogDebug(plans.Message);
                    return BadRequest(plans);
                }

                return Ok(plans);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error fetching plans in project.");
                throw;
            }
        }

        /// <summary>
        /// Retrieves detailed information about a specific plan by its ID.
        /// </summary>
        /// <param name="token">JWT token for authentication.</param>
        /// <param name="planId">ID of the plan.</param>
        /// <returns>Details of the specified plan.</returns>
        [HttpGet("{planId}")]
        public async Task<IActionResult> GetDetailPlanById(string token, string planId)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(planId))
                {
                    _loggerHelper.LogWarning("Token or Plan ID is null or empty.");
                    return BadRequest("Token and Plan ID are required.");
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

                var plan = await _planLogic.GetDetailPlanById(user.Data.UserId, planId);
                if (!plan.Status)
                {
                    _loggerHelper.LogDebug(plan.Message);
                    return BadRequest(plan);
                }

                return Ok(plan);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error fetching plan details.");
                throw;
            }
        }

        #endregion

        #region Create Methods

        /// <summary>
        /// Adds a new plan to a specific project.
        /// </summary>
        /// <param name="token">JWT token for authentication.</param>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="plan">Details of the plan to be added.</param>
        /// <returns>Status of the creation operation.</returns>
        [HttpPost("project/{projectId}")]
        public async Task<IActionResult> AddNewPlan(string token, string projectId, AddPlan plan)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(projectId) || plan == null || !ModelState.IsValid)
                {
                    _loggerHelper.LogWarning("Invalid input for adding a new plan.");
                    return BadRequest("Invalid input parameters.");
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

                var response = await _planLogic.Add(user.Data.UserId, projectId, plan);
                if (!response.Status)
                {
                    _loggerHelper.LogDebug(response.Message);
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error adding a new plan.");
                throw;
            }
        }

        #endregion

        #region Update Methods

        /// <summary>
        /// Updates the details of a specific plan.
        /// </summary>
        /// <param name="token">JWT token for authentication.</param>
        /// <param name="planId">ID of the plan.</param>
        /// <param name="plan">Updated details of the plan.</param>
        /// <returns>Status of the update operation.</returns>
        [HttpPut("{planId}")]
        public async Task<IActionResult> UpdateInfo(string token, string planId, UpdatePlan plan)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(planId) || plan == null || !ModelState.IsValid)
                {
                    _loggerHelper.LogWarning("Invalid input for updating a plan.");
                    return BadRequest("Invalid input parameters.");
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

                var response = await _planLogic.UpdateInfo(user.Data.UserId, planId, plan);
                if (!response.Status)
                {
                    _loggerHelper.LogDebug(response.Message);
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error updating the plan.");
                throw;
            }
        }

        /// <summary>
        /// Marks a specific plan as done.
        /// </summary>
        /// <param name="token">JWT token for authentication.</param>
        /// <param name="planId">ID of the plan.</param>
        /// <returns>Status of the operation.</returns>
        [HttpPut("{planId}/done")]
        public async Task<IActionResult> UpdateIsDone(string token, string planId)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(planId))
                {
                    _loggerHelper.LogWarning("Invalid input for marking a plan as done.");
                    return BadRequest("Invalid input parameters.");
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

                var response = await _planLogic.UpdateIsDone(user.Data.UserId, planId);
                if (!response.Status)
                {
                    _loggerHelper.LogDebug(response.Message);
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error marking the plan as done.");
                throw;
            }
        }

        #endregion
    }
}
