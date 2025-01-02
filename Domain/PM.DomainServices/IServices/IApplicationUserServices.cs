using PM.Domain;
using System.Numerics;

namespace PM.Persistence.IServices
{
    /// <summary>
    /// Service interface for managing application user operations.
    /// </summary>
    public interface IApplicationUserServices
    {
        #region Authentication
        /// <summary>
        /// Logs in a user with the specified email and password.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>True if login was successful; otherwise, false.</returns>
        Task<bool> Login(string email, string password);

        /// <summary>
        /// Logs out the currently authenticated user.
        /// </summary>
        /// <param name="userId">The ID of the user to log out.</param>
        /// <returns>True if logout was successful; otherwise, false.</returns>
        Task<bool> Logout(string userId);
        #endregion

        #region User Registration
        /// <summary>
        /// Registers a new user with the "User" role.
        /// </summary>
        /// <param name="userName">The username of the new user.</param>
        /// <param name="email">The email address of the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <returns>True if registration was successful; otherwise, false.</returns>
        Task<bool> RegisterUser(string userName, string email, string password);

        /// <summary>
        /// Registers a new admin with the "Admin" role.
        /// </summary>
        /// <param name="userName">The username of the new admin.</param>
        /// <param name="email">The email address of the new admin.</param>
        /// <param name="password">The password for the new admin.</param>
        /// <returns>True if registration was successful; otherwise, false.</returns>
        Task<bool> RegisterAdmin(string userName, string email, string password);
        #endregion

        #region User Management
        /// <summary>
        /// Retrieves user details by user ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The user details, or null if the user does not exist.</returns>
        Task<ApplicationUser> GetUserDetailByUserId(string userId);

        /// <summary>
        /// Retrieves user details by email.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <returns>The user details, or null if the user does not exist.</returns>
        Task<ApplicationUser> GetUserDetailByMail(string email);
        #endregion

        #region Role Management
        /// <summary>
        /// Retrieves the role of a user by their email.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <returns>The role of the user, or an empty string if no role is found.</returns>
        Task<string> GetRoleByEmail(string email);
        #endregion
    }
}
