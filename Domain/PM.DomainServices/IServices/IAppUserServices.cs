using PM.DomainServices.Models;
using PM.DomainServices.Models.users;

namespace PM.Persistence.IServices
{
    /// <summary>
    /// Service interface for managing application user operations.
    /// </summary>
    public interface IApplicationUserServices
    {
        /// <summary>
        /// Retrieves detailed information about an application user based on their ID or email.
        /// </summary>
        /// <param name="text">The ID or email of the user to be retrieved.</param>
        /// <returns>A <see cref="ServicesResult{T}"/> containing user details if successful, or an error message if not.</returns>
        Task<ServicesResult<DetailAppUser>> GetAppUserByIdOrEmail(string text);

        /// <summary>
        /// Updates the information of an application user.
        /// </summary>
        /// <param name="userId">The ID of the user whose information is to be updated.</param>
        /// <param name="updateAppUser">An object containing the updated user information.</param>
        /// <returns>A <see cref="ServicesResult{T}"/> indicating the success or failure of the update operation.</returns>
        Task<ServicesResult<bool>> UpdateInfoUser(string userId, UpdateAppUser updateAppUser);
    }
}
