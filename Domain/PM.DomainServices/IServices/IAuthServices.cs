using PM.Domain;
using PM.DomainServices.Models;

namespace PM.DomainServices.IServices
{
    /// <summary>
    /// Service interface for handling authentication-related operations such as login, logout, and user registration.
    /// </summary>
    public interface IAuthServices
    {
        /// <summary>
        /// Authenticates a user by validating their email and password.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>
        /// A <see cref="ServicesResult{T}"/> containing the authenticated <see cref="ApplicationUser"/> if successful, or an error message if not.
        /// </returns>
        Task<ServicesResult<ApplicationUser>> Login(string email, string password);

        /// <summary>
        /// Logs out the currently authenticated user.
        /// </summary>
        /// <param name="userId">The ID of the user to log out.</param>
        /// <returns>
        /// A <see cref="ServicesResult{T}"/> indicating whether the logout operation was successful.
        /// </returns>
        Task<ServicesResult<bool>> Logout(string userId);

        /// <summary>
        /// Registers a new user with the given email, username, and password.
        /// </summary>
        /// <param name="email">The email address for the new user.</param>
        /// <param name="username">The username for the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <returns>
        /// A <see cref="ServicesResult{T}"/> indicating whether the registration was successful.
        /// </returns>
        Task<ServicesResult<bool>> Register(string email, string username, string password);
    }
}
