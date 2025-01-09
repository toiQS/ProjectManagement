using PM.DomainServices.Models;
using PM.DomainServices.Models.projects;
using PM.DomainServices.Models.users;

namespace PM.DomainServices.ILogic
{
    /// <summary>
    /// Interface defining project-related operations and management logic.
    /// </summary>
    public interface IProjectLogic
    {
        #region User Management

        /// <summary>
        /// Verifies if a user exists in the database. 
        /// If the user exists, their details are retrieved to allow further actions.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A service result containing user details.</returns>
        Task<ServicesResult<DetailAppUser>> CheckAndGetUser(string userId);

        #endregion

        #region Project Retrieval

        /// <summary>
        /// Retrieves a list of projects that the user has joined.
        /// </summary>
        /// <returns>A service result containing a collection of projects the user has joined.</returns>
        Task<ServicesResult<IEnumerable<IndexProject>>> GetListProjectsUserHasJoined();

        /// <summary>
        /// Retrieves a list of projects where the user is the owner.
        /// </summary>
        /// <returns>A service result containing a collection of projects owned by the user.</returns>
        Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectListUserHasOwner();

        /// <summary>
        /// Retrieves detailed information about a specific project that the user has joined.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>A service result containing detailed project information.</returns>
        Task<ServicesResult<DetailProject>> GetProjectDetailProjectHasJoined(string projectId);

        #endregion

        #region Project Management

        /// <summary>
        /// Adds a new project to the system.
        /// </summary>
        /// <param name="addProject">The project details to be added.</param>
        /// <returns>A service result indicating the success or failure of the operation.</returns>
        Task<ServicesResult<bool>> Add(AddProject addProject);

        /// <summary>
        /// Updates information about an existing project.
        /// </summary>
        /// <param name="projectId">The ID of the project to be updated.</param>
        /// <param name="updateProject">The updated project details.</param>
        /// <returns>A service result indicating the success or failure of the operation.</returns>
        Task<ServicesResult<bool>> UpdateInfo(string projectId, UpdateProject updateProject);

        /// <summary>
        /// Marks a project as deleted in the system.
        /// </summary>
        /// <param name="projectId">The ID of the project to be marked as deleted.</param>
        /// <returns>A service result indicating the success or failure of the operation.</returns>
        Task<ServicesResult<bool>> UpdateIsDeletedAsync(string projectId);

        /// <summary>
        /// Updates the "accessed" status of a project for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>A service result indicating the success or failure of the operation.</returns>
        Task<ServicesResult<bool>> UpdateIsAccessedAsync(string userId, string projectId);

        /// <summary>
        /// Marks a project as completed.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>A service result indicating the success or failure of the operation.</returns>
        Task<ServicesResult<bool>> UpdateIsDoneAsync(string userId, string projectId);

        #endregion
    }
}
