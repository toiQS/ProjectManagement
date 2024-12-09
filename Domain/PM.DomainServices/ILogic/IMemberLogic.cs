using Shared;
using Shared.member;

namespace PM.DomainServices.ILogic
{
    /// <summary>
    /// Defines the contract for member-related logic in a project management system.
    /// </summary>
    public interface IMemberLogic
    {
        #region Member Retrieval

        /// <summary>
        /// Retrieves all members in a specific project.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the members.</param>
        /// <param name="projectId">The ID of the project to retrieve members from.</param>
        /// <returns>A result containing a list of members or an error message.</returns>
        Task<ServicesResult<IEnumerable<IndexMember>>> GetMembersInProject(string userId, string projectId);

        /// <summary>
        /// Retrieves detailed information about a specific member by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the member details.</param>
        /// <param name="memberId">The ID of the member to retrieve.</param>
        /// <returns>A result containing the member's details or an error message.</returns>
        Task<ServicesResult<DetailMember>> GetMemberByMemberId(string userId, string memberId);

        #endregion

        #region Member Management

        /// <summary>
        /// Adds a new member to a project.
        /// </summary>
        /// <param name="userId">The ID of the user adding the member.</param>
        /// <param name="addMember">The data for the member to add.</param>
        /// <param name="projectId">The ID of the project to add the member to.</param>
        /// <returns>A result indicating the success or failure of the operation.</returns>
        Task<ServicesResult<bool>> Add(string userId, AddMember addMember, string projectId);

        /// <summary>
        /// Updates information for an existing member.
        /// </summary>
        /// <param name="userId">The ID of the user updating the member information.</param>
        /// <param name="memberId">The ID of the member to update.</param>
        /// <param name="member">The updated member information.</param>
        /// <returns>A result indicating the success or failure of the update operation.</returns>
        Task<ServicesResult<bool>> UpdateInfo(string userId, string memberId, UpdateMember member);

        /// <summary>
        /// Deletes a member from the project.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the deletion.</param>
        /// <param name="memberId">The ID of the member to delete.</param>
        /// <returns>A result indicating the success or failure of the deletion operation.</returns>
        Task<ServicesResult<bool>> Delete(string userId, string memberId);

        #endregion
    }
}
