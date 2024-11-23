using PM.Persistence.IServices;
using PM.DomainServices.Shared;
using PM.Domain;
using PM.DomainServices.ILogic;

namespace PM.DomainServices.Logic
{
    /// <summary>
    /// Handles business logic for user-related operations such as login, registration, and role management.
    /// </summary>
    public class UserLogic : IUserLogic
    {
        private readonly IApplicationUserServices _userServices;

        /// <summary>
        /// Constructor to inject dependencies for user services.
        /// </summary>
        /// <param name="applicationUserServices">The service for managing application users.</param>
        public UserLogic(IApplicationUserServices applicationUserServices)
        {
            _userServices = applicationUserServices;
        }

        /// <summary>
        /// Handles the logic for user login.
        /// </summary>
        /// <param name="email">The email address of the user attempting to log in.</param>
        /// <param name="password">The password of the user attempting to log in.</param>
        /// <returns>
        /// A <see cref="ServicesResult{T}"/> containing the logged-in user object if successful,
        /// or an error message if the login attempt fails.
        /// </returns>
        public async Task<ServicesResult<ApplicationUser>> Login(string email, string password)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return ServicesResult<ApplicationUser>.Failure("Email and password must be provided.");
            }

            // Attempt to log in using the user services
            var user = await _userServices.LoginServices(email, password);
            if (user != null)
            {
                return ServicesResult<ApplicationUser>.Success(user);
            }

            // Return failure if login attempt is unsuccessful
            return ServicesResult<ApplicationUser>.Failure("Login failed. Invalid email or password.");
        }

        /// <summary>
        /// Handles the logic for user registration.
        /// </summary>
        /// <param name="userName">The desired username for the new user.</param>
        /// <param name="email">The email address for the new user.</param>
        /// <param name="password">The password for the new user account.</param>
        /// <returns>
        /// A <see cref="ServicesResult{T}"/> indicating the success or failure of the registration.
        /// </returns>
        public async Task<ServicesResult<bool>> RegisterUser(string userName, string email, string password)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return ServicesResult<bool>.Failure("Username, email, and password must be provided.");
            }

            // Attempt to register the user
            bool isRegistered = await _userServices.RegisterApplicationUser(userName, email, password);
            if (isRegistered)
            {
                return ServicesResult<bool>.Success(true);
            }

            // Return failure if registration is unsuccessful
            return ServicesResult<bool>.Failure("Registration failed.");
        }

        /// <summary>
        /// Retrieves the role name of a user by their user ID.
        /// </summary>
        /// <param name="userId">The ID of the user whose role is being requested.</param>
        /// <returns>
        /// A <see cref="ServicesResult{T}"/> containing the role name if successful,
        /// or an error message if the operation fails.
        /// </returns>
        public async Task<ServicesResult<string>> GetRoleNameOfUser(string userId)
        {
            // Validate input parameter
            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<string>.Failure("User ID must be provided.");
            }

            // Attempt to retrieve the role name using user services
            var role = await _userServices.GetRoleApplicatonUserByUserIdAsync(userId);
            if (role != null)
            {
                return ServicesResult<string>.Success(role);
            }

            // Return failure if role retrieval is unsuccessful
            return ServicesResult<string>.Failure("Failed to retrieve user role.");
        }
    }
}
