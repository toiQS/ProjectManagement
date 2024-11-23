using PM.Persistence.IServices;
using PM.DomainServices.Shared;
using PM.Domain;

namespace PM.DomainServices.Logic
{
    /// <summary>
    /// Handles business logic for user-related operations, such as login and registration.
    /// </summary>
    public class UserLogic
    {
        private readonly IApplicationUserServices _userServices;

        /// <summary>
        /// Constructor to initialize dependencies for user services.
        /// </summary>
        /// <param name="applicationUserServices">The service for user operations.</param>
        public UserLogic(IApplicationUserServices applicationUserServices)
        {
            _userServices = applicationUserServices;
        }

        /// <summary>
        /// Handles user login logic.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A service result containing the logged-in user object if successful, or an error message if failed.</returns>
        public async Task<ServicesResult<ApplicationUser>> Login(string email, string password)
        {
            // Validate input
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return ServicesResult<ApplicationUser>.Failure("Email and password must be provided.");
            }

            // Attempt to log in using the user service
            var user = await _userServices.LoginServices(email, password);
            if (user != null)
            {
                return ServicesResult<ApplicationUser>.Success(user);
            }

            // Return failure if the login attempt was unsuccessful
            return ServicesResult<ApplicationUser>.Failure("Login failed. Invalid email or password.");
        }

        /// <summary>
        /// Handles user registration logic.
        /// </summary>
        /// <param name="userName">The username of the new user.</param>
        /// <param name="email">The email address of the new user.</param>
        /// <param name="password">The password for the new user account.</param>
        /// <returns>A service result indicating success or failure of the registration.</returns>
        public async Task<ServicesResult<bool>> RegisterUser(string userName, string email, string password)
        {
            // Validate input
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

            // Return failure if registration was unsuccessful
            return ServicesResult<bool>.Failure("Registration failed.");
        }
        #region get role name of user
        public async Task<ServicesResult<string>> GetRoleNameOfUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<string>.Failure("");
            }
            var roleName = await _userServices.GetRoleApplicatonUserByUserIdAsync(userId);
            if (roleName != null) return ServicesResult<string>.Success(roleName);
            return ServicesResult<string>.Failure($"{userId} is not registered");
        }
        #endregion
    }
}
