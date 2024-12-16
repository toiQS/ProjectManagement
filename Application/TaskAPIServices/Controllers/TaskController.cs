using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Domain;
using PM.DomainServices.ILogic;
using PM.Infrastructure.Jwts;
using PM.Infrastructure.Loggers;
using Shared.task;
using System;
using System.Threading.Tasks;

namespace TaskAPIServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        #region Initialize logic services
        private readonly IAuthLogic _authLogic;
        private readonly ITaskLogic _taskLogic;
        private readonly IJwtHelper _jwtHelper;
        private readonly ILoggerHelper<TaskController> _loggerHelper;

        public TaskController(IAuthLogic authLogic, ITaskLogic taskLogic, IJwtHelper jwtHelper, ILoggerHelper<TaskController> loggerHelper)
        {
            _authLogic = authLogic;
            _taskLogic = taskLogic;
            _jwtHelper = jwtHelper;
            _loggerHelper = loggerHelper;
        }
        #endregion

        #region Task Retrieval Methods

        /// <summary>
        /// Get a list of tasks within a specific plan.
        /// </summary>
        /// <param name="token">JWT token for authentication.</param>
        /// <param name="planId">Plan ID to retrieve tasks.</param>
        /// <returns>List of tasks in the specified plan.</returns>
        [HttpGet("tasks/plan")]
        public async Task<IActionResult> GetTaskListInPlan(string token, string planId)
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

                var tasks = await _taskLogic.GetTaskListInPlan(user.Data.UserId, planId);
                if (!tasks.Status)
                {
                    _loggerHelper.LogDebug(tasks.Message);
                    return BadRequest(tasks);
                }

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error fetching task list in plan.");
                throw;
            }
        }

        /// <summary>
        /// Get detailed information about a specific task.
        /// </summary>
        /// <param name="token">JWT token for authentication.</param>
        /// <param name="taskId">Task ID to retrieve details.</param>
        /// <returns>Details of the specified task.</returns>
        [HttpGet("tasks/detail")]
        public async Task<IActionResult> GetTaskDetail(string token, string taskId)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(taskId))
                {
                    _loggerHelper.LogWarning("Token or Task ID is null or empty.");
                    return BadRequest("Token and Task ID are required.");
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

                var task = await _taskLogic.GetTaskDetail(user.Data.UserId, taskId);
                if (!task.Status)
                {
                    _loggerHelper.LogDebug(task.Message);
                    return BadRequest(task);
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error fetching task details.");
                throw;
            }
        }

        #endregion

        #region Task Management Methods

        /// <summary>
        /// Add a new task to a specific plan.
        /// </summary>
        /// <param name="token">JWT token for authentication.</param>
        /// <param name="planId">Plan ID to which the task will be added.</param>
        /// <param name="addTask">Task details.</param>
        /// <returns>Result of task creation.</returns>
        [HttpPost("tasks/add")]
        public async Task<IActionResult> AddNewTask(string token, string planId, AddTask addTask)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(planId) || addTask == null || !ModelState.IsValid)
                {
                    _loggerHelper.LogWarning("Token, Plan ID, or task details are invalid.");
                    return BadRequest("Token, Plan ID, and valid task details are required.");
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

                var taskResult = await _taskLogic.Add(user.Data.UserId, addTask, planId);
                if (!taskResult.Status)
                {
                    _loggerHelper.LogDebug(taskResult.Message);
                    return BadRequest(taskResult);
                }

                return Ok(taskResult);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error adding new task.");
                throw;
            }
        }

        /// <summary>
        /// Update the information of an existing task.
        /// </summary>
        /// <param name="token">JWT token for authentication.</param>
        /// <param name="taskId">Task ID to be updated.</param>
        /// <param name="updateTask">Updated task details.</param>
        /// <returns>Result of task update.</returns>
        [HttpPut("tasks/update")]
        public async Task<IActionResult> UpdateTaskInfo(string token, string taskId, UpdateTask updateTask)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(taskId) || updateTask == null || !ModelState.IsValid)
                {
                    _loggerHelper.LogWarning("Token, Task ID, or task details are invalid.");
                    return BadRequest("Token, Task ID, and valid task details are required.");
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

                var updateResult = await _taskLogic.UpdateInfo(user.Data.UserId, taskId, updateTask);
                if (!updateResult.Status)
                {
                    _loggerHelper.LogDebug(updateResult.Message);
                    return BadRequest(updateResult);
                }

                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error updating task information.");
                throw;
            }
        }

        #endregion

        #region Task Status Management

        /// <summary>
        /// Update the status of a task.
        /// </summary>
        /// <param name="token">JWT token for authentication.</param>
        /// <param name="taskId">Task ID to update status.</param>
        /// <returns>Result of status update.</returns>
        [HttpPut("tasks/status")]
        public async Task<IActionResult> UpdateTaskStatus(string token, string taskId)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(taskId))
                {
                    _loggerHelper.LogWarning("Token or Task ID is null or empty.");
                    return BadRequest("Token and Task ID are required.");
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

                var statusResult = await _taskLogic.UpdateStatus(user.Data.UserId, taskId);
                if (!statusResult.Status)
                {
                    _loggerHelper.LogDebug(statusResult.Message);
                    return BadRequest(statusResult);
                }

                return Ok(statusResult);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error updating task status.");
                throw;
            }
        }

        #endregion
    }
}
