using Microsoft.AspNetCore.Identity;
using PM.Domain;
using PM.Persistence.IServices;

namespace PM.Persistence.Services
{
    /// <summary>
    /// Implements user-related services for authentication, registration, role management, and logout.
    /// </summary>
    public class ApplicationUserServices : IApplicationUserServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        /// <summary>
        /// Constructor to initialize the user, role, and sign-in managers.
        /// </summary>
        public ApplicationUserServices(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        #region Register User
        /// <summary>
        /// Registers a new user and assigns them the "User" role.
        /// </summary>
        /// <param name="userName">The username of the new user.</param>
        /// <param name="email">The email of the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <returns>A boolean indicating whether the registration was successful.</returns>
        public async Task<bool> RegisterApplicationUser(string userName, string email, string password)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email
                };

                // Create the user with the provided password
                var createResult = await _userManager.CreateAsync(user, password);
                if (!createResult.Succeeded) return false;

                // Assign the "User" role to the new user
                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                return roleResult.Succeeded;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region Register Admin
        /// <summary>
        /// Registers a new administrator and assigns them the "Admin" role.
        /// </summary>
        /// <param name="userName">The username of the new admin.</param>
        /// <param name="email">The email of the new admin.</param>
        /// <param name="password">The password for the new admin.</param>
        /// <returns>A boolean indicating whether the registration was successful.</returns>
        public async Task<bool> RegisterApplicationAdmin(string userName, string email, string password)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email
                };

                // Create the user with the provided password
                var createResult = await _userManager.CreateAsync(user, password);
                if (!createResult.Succeeded) return false;

                // Assign the "Admin" role to the new admin
                var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
                return roleResult.Succeeded;
            }
            catch (Exception)
            {
                throw; // Rethrow the exception for higher-level handling
            }
        }
        #endregion

        #region Login
        /// <summary>
        /// Authenticates a user with the provided email and password.
        /// </summary>
        /// <param name="email">The email of the user attempting to log in.</param>
        /// <param name="password">The password of the user attempting to log in.</param>
        /// <returns>The authenticated user object, or null if authentication fails.</returns>
        public async Task<ApplicationUser> LoginServices(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null && await _userManager.CheckPasswordAsync(user, password))
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return user;
                }
                return null;
            }
            catch (Exception)
            {
                throw; // Rethrow the exception for higher-level handling
            }
        }
        #endregion

        #region Get Role of User
        /// <summary>
        /// Retrieves the role of a user by their user ID.
        /// </summary>
        /// <param name="userId">The ID of the user whose role is being retrieved.</param>
        /// <returns>The name of the user's role, or null if the user is not found.</returns>
        public async Task<string> GetRoleApplicatonUserByUserIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return null;

                var roles = await _userManager.GetRolesAsync(user);
                return roles.FirstOrDefault(); // Return the first role, assuming a single-role system
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // Log the exception for debugging purposes
                return null;
            }
        }
        #endregion

        #region Logout
        /// <summary>
        /// Logs out the currently signed-in user.
        /// </summary>
        public async Task Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
            }
            catch (Exception)
            {
                throw; // Rethrow the exception for higher-level handling
            }
        }
        #endregion
        #region get user by user id
        public Task<ApplicationUser> GetUser(string userId)
        {
            try
            {
                return _userManager.FindByIdAsync(userId);
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}
