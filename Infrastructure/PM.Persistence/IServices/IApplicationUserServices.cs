using PM.Domain;

namespace PM.Persistence.IServices
{
    /// <summary>
    /// Service interface for managing application user operations.
    /// </summary>
    public interface IApplicationUserServices
    {
        #region User Registration

        /// <summary>
        /// Registers a new application user.
        /// </summary>
        /// <param name="userName">The username of the new user.</param>
        /// <param name="email">The email address of the new user.</param>
        /// <param name="password">The password of the new user.</param>
        /// <returns>Returns true if the registration is successful, otherwise false.</returns>
        Task<bool> RegisterApplicationUser(string userName, string email, string password);

        /// <summary>
        /// Registers a new application admin user.
        /// </summary>
        /// <param name="userName">The username of the admin user.</param>
        /// <param name="email">The email address of the admin user.</param>
        /// <param name="password">The password of the admin user.</param>
        /// <returns>Returns true if the registration is successful, otherwise false.</returns>
        Task<bool> RegisterApplicationAdmin(string userName, string email, string password);

        #endregion

        #region User Authentication

        /// <summary>
        /// Authenticates a user using their email and password.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>Returns the authenticated <see cref="ApplicationUser"/> if successful, otherwise null.</returns>
        Task<ApplicationUser> LoginServices(string email, string password);

        /// <summary>
        /// Logs out a user by their user ID.
        /// </summary>
        /// <param name="userId">The ID of the user to log out.</param>
        /// <returns>Returns true if the logout is successful, otherwise false.</returns>
        Task<bool> Logout(string userId);

        #endregion

        #region User Role Management

        /// <summary>
        /// Retrieves the role of an application user by their user ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>Returns the role name as a string.</returns>
        Task<string> GetRoleApplicatonUserByUserIdAsync(string userId);

        #endregion

        #region User Retrieval

        /// <summary>
        /// Retrieves user information by their user ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>Returns the <see cref="ApplicationUser"/> object.</returns>
        Task<ApplicationUser> GetUser(string userId);

        #endregion
        #region Get a user specific  by email
        /// <summary>
        /// Retrieves user information by their user email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Returns the <see cref="ApplicationUser"/></returns>
        Task<ApplicationUser> GetUserByEmail(string email);
        #endregion
    }
}
