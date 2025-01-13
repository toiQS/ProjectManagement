using PM.DomainServices.Models;
using PM.DomainServices.Models.members;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PM.DomainServices.ILogic
{
    /// <summary>
    /// Interface for member-related business logic operations in the project management domain.
    /// Provides methods to fetch information about project members, their roles, and projects they are involved in.
    /// </summary>
    public interface IMemberLogic
    {
        /// <summary>
        /// Retrieves the role of a specific member within a project.
        /// </summary>
        /// <param name="memberId">The unique ID of the member.</param>
        /// <returns>A service result containing the role name of the member, or an error message.</returns>
        Task<ServicesResult<string>> GetRoleMemberInProjectAsync(string memberId);

        /// <summary>
        /// Retrieves information about the owner of a specific project.
        /// </summary>
        /// <param name="projectId">The unique ID of the project.</param>
        /// <returns>A service result containing the owner's information, or an error message.</returns>
        Task<ServicesResult<IndexMember>> GetInfoOfOwnerInProjectAsync(string projectId);

        /// <summary>
        /// Retrieves a list of members for a specific project.
        /// </summary>
        /// <param name="projectId">The unique ID of the project.</param>
        /// <returns>A service result containing a list of members, or an error message.</returns>
        Task<ServicesResult<IEnumerable<IndexMember>>> GetMembersInProjectAsync(string projectId);

        /// <summary>
        /// Retrieves a list of projects that a user has joined, based on their user ID.
        /// </summary>
        /// <param name="userId">The unique ID of the user.</param>
        /// <returns>A service result containing a list of project IDs, or an error message.</returns>
        Task<ServicesResult<IEnumerable<string>>> GetProjectsUserHasJoinedByUserIdAsync(string userId);

        /// <summary>
        /// Retrieves a list of projects that a user owns, based on their user ID.
        /// </summary>
        /// <param name="userId">The unique ID of the user.</param>
        /// <returns>A service result containing a list of project IDs, or an error message.</returns>
        Task<ServicesResult<IEnumerable<string>>> GetProjectsUserOwnsAsync(string userId);

        /// <summary>
        /// Retrieves a list of all members in the system.
        /// </summary>
        /// <returns>A service result containing a list of all members, or an error message.</returns>
        Task<ServicesResult<IEnumerable<IndexMember>>> GetAllAsync();
    }
}
