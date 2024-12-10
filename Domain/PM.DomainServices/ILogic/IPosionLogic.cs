using Shared;
using Shared.position;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PM.DomainServices.ILogic
{
    /// <summary>
    /// Interface defining operations and management logic for positions within a project.
    /// </summary>
    public interface IPositionLogic
    {
        /// <summary>
        /// Retrieves a list of positions associated with a specific project.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the positions.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>A service result containing a list of positions in the project.</returns>
        Task<ServicesResult<IEnumerable<IndexPosition>>> Get(string userId, string projectId);

        /// <summary>
        /// Adds a new position to a specific project.
        /// </summary>
        /// <param name="userId">The ID of the user adding the position.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="position">The details of the position to be added.</param>
        /// <returns>A service result indicating the success or failure of the add operation.</returns>
        Task<ServicesResult<bool>> Add(string userId, string projectId, AddPosition position);

        /// <summary>
        /// Deletes a specific position from a project.
        /// </summary>
        /// <param name="userId">The ID of the user performing the deletion.</param>
        /// <param name="positionId">The ID of the position to delete.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>A service result indicating the success or failure of the delete operation.</returns>
        Task<ServicesResult<bool>> Delete(string userId, string positionId, string projectId);

        /// <summary>
        /// Updates the details of an existing position in a project.
        /// </summary>
        /// <param name="userId">The ID of the user performing the update.</param>
        /// <param name="positionId">The ID of the position to update.</param>
        /// <param name="position">The updated position details.</param>
        /// <returns>A service result indicating the success or failure of the update operation.</returns>
        Task<ServicesResult<bool>> Update(string userId, string positionId, UpdatePositon position);
    }
}
