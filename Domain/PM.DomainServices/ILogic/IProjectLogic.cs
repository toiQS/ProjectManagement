using Shared;
using Shared.project;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PM.DomainServices.ILogic
{
    /// <summary>
    /// Interface defining project-related operations and management logic.
    /// </summary>
    public interface IProjectLogic
    {
        #region Project Retrieval

        /// <summary>
        /// Retrieves a list of projects that the user has joined.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A service result containing a list of projects the user has joined.</returns>
        Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectListUserHasJoined(string userId);

        /// <summary>
        /// Retrieves a list of projects that the user owns.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A service result containing a list of projects owned by the user.</returns>
        Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectListUserHasOwner(string userId);

        /// <summary>
        /// Retrieves detailed information about a project the user has joined.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>A service result containing the project's detailed information.</returns>
        Task<ServicesResult<DetailProject>> GetProjectDetailProjectHasJoined(string userId, string projectId);

        #endregion

        #region Project Management

        /// <summary>
        /// Adds a new project associated with a user.
        /// </summary>
        /// <param name="userId">The ID of the user adding the project.</param>
        /// <param name="addProject">The project details to add.</param>
        /// <returns>A service result indicating the success or failure of the add operation.</returns>
        Task<ServicesResult<bool>> Add(string userId, AddProject addProject);

        /// <summary>
        /// Updates the information of an existing project.
        /// </summary>
        /// <param name="userId">The ID of the user performing the update.</param>
        /// <param name="projectId">The ID of the project to update.</param>
        /// <param name="updateProject">The updated project details.</param>
        /// <returns>A service result indicating the success or failure of the update operation.</returns>
        Task<ServicesResult<bool>> UpdateInfo(string userId, string projectId, UpdateProject updateProject);

        /// <summary>
        /// Deletes a project associated with a user.
        /// </summary>
        /// <param name="userId">The ID of the user performing the deletion.</param>
        /// <param name="projectId">The ID of the project to delete.</param>
        /// <returns>A service result indicating the success or failure of the deletion operation.</returns>
        Task<ServicesResult<bool>> DeleteProjectAsync(string userId, string projectId);

        #endregion

        #region Project Status Updates

        /// <summary>
        /// Updates the "IsDeleted" status of a project.
        /// </summary>
        /// <param name="userId">The ID of the user performing the operation.</param>
        /// <param name="projectId">The ID of the project to update.</param>
        /// <returns>A service result indicating the success or failure of the operation.</returns>
        Task<ServicesResult<bool>> UpdateIsDeletedAsync(string userId, string projectId);

        /// <summary>
        /// Updates the "IsAccessed" status of a project.
        /// </summary>
        /// <param name="userId">The ID of the user performing the operation.</param>
        /// <param name="projectId">The ID of the project to update.</param>
        /// <returns>A service result indicating the success or failure of the operation.</returns>
        Task<ServicesResult<bool>> UpdateIsAccessedAsync(string userId, string projectId);

        /// <summary>
        /// Updates the "IsDone" status of a project.
        /// </summary>
        /// <param name="userId">The ID of the user performing the operation.</param>
        /// <param name="projectId">The ID of the project to update.</param>
        /// <returns>A service result indicating the success or failure of the operation.</returns>
        Task<ServicesResult<bool>> UpdateIsDoneAsync(string userId, string projectId);

        /// <summary>
        /// Updates the status of a project.
        /// </summary>
        /// <param name="userId">The ID of the user performing the operation.</param>
        /// <param name="projectId">The ID of the project to update.</param>
        /// <returns>A service result indicating the success or failure of the operation.</returns>
        Task<ServicesResult<bool>> UpdateProjectStatusAsync(string userId, string projectId);

        #endregion
    }
}
