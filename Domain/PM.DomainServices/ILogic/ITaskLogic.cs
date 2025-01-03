using PM.DomainServices.Models;
using PM.DomainServices.Models.tasks;

namespace PM.DomainServices.ILogic
{
    /// <summary>
    /// Interface defining task-related operations and management logic.
    /// </summary>
    public interface ITaskLogic
    {
        #region Task Retrieval

        /// <summary>
        /// Retrieves a list of tasks within a specific plan.
        /// </summary>
        /// <param name="userId">The ID of the user retrieving the tasks.</param>
        /// <param name="planId">The ID of the plan containing the tasks.</param>
        /// <returns>A service result containing a list of tasks in the plan.</returns>
        Task<ServicesResult<IEnumerable<IndexTask>>> GetTaskListInPlan(string userId, string planId);

        /// <summary>
        /// Retrieves detailed information about a specific task.
        /// </summary>
        /// <param name="userId">The ID of the user retrieving the task details.</param>
        /// <param name="taskId">The ID of the task to retrieve.</param>
        /// <returns>A service result containing the task details.</returns>
        Task<ServicesResult<DetailTask>> GetTaskDetail(string userId, string taskId);

        #endregion

        #region Task Management

        /// <summary>
        /// Adds a new task associated with a user.
        /// </summary>
        /// <param name="userId">The ID of the user adding the task.</param>
        /// <param name="addTask">The task details to add.</param>
        /// <param name="planId">The ID of the plan adding the task</param>
        /// <returns>A service result indicating the success or failure of the add operation.</returns>
        Task<ServicesResult<bool>> Add(string userId, AddTask addTask, string planId);

        /// <summary>
        /// Updates the information of an existing task.
        /// </summary>
        /// <param name="userId">The ID of the user performing the update.</param>
        /// <param name="updateId">The ID of the task to update.</param>
        /// <param name="updateTask">The updated task details.</param>
        /// <returns>A service result indicating the success or failure of the update operation.</returns>
        Task<ServicesResult<bool>> UpdateInfo(string userId, string updateId, UpdateTask updateTask);

        /// <summary>
        /// Deletes a task associated with a user.
        /// </summary>
        /// <param name="userId">The ID of the user performing the deletion.</param>
        /// <param name="taskId">The ID of the task to delete.</param>
        /// <returns>A service result indicating the success or failure of the deletion operation.</returns>
        Task<ServicesResult<bool>> Delete(string userId, string taskId);

        #endregion

        #region Task Status Updates

        /// <summary>
        /// Updates the status of a specific task.
        /// </summary>
        /// <param name="userId">The ID of the user performing the update.</param>
        /// <param name="taskId">The ID of the task to update.</param>
        /// <returns>A service result indicating the success or failure of the status update.</returns>
        Task<ServicesResult<bool>> UpdateStatus(string userId, string taskId);

        /// <summary>
        /// Marks a specific task as done.
        /// </summary>
        /// <param name="userId">The ID of the user marking the task as done.</param>
        /// <param name="taskId">The ID of the task to mark as done.</param>
        /// <returns>A service result indicating the success or failure of the operation.</returns>
        Task<ServicesResult<bool>> UpdateIsDone(string userId, string taskId);

        #endregion
    }
}
