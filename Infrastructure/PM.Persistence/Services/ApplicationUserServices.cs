using PM.Domain;
using System.Numerics;
using Microsoft.AspNetCore.Identity;
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
        /// Initializes the user, role, and sign-in managers.
        /// </summary>
        /// <param name="userManager">Manages user-related operations.</param>
        /// <param name="roleManager">Manages role-related operations.</param>
        /// <param name="signInManager">Manages user sign-in operations.</param>
        public ApplicationUserServices(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        #region User Registration
        /// <summary>
        /// Registers a new user with the "User" role.
        /// </summary>
        /// <param name="userName">The username of the new user.</param>
        /// <param name="email">The email address of the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <returns>True if the registration was successful; otherwise, false.</returns>
        public async Task<bool> RegisterUser(string userName, string email, string password)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email
                };

                var createResult = await _userManager.CreateAsync(user, password);
                if (!createResult.Succeeded) return false;

                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                return roleResult.Succeeded;
            }
            catch (Exception ex)
            {
                // Log the exception (example: use a logging framework)
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Registers a new admin with the "Admin" role.
        /// </summary>
        /// <param name="userName">The username of the new admin.</param>
        /// <param name="email">The email address of the new admin.</param>
        /// <param name="password">The password for the new admin.</param>
        /// <returns>True if the registration was successful; otherwise, false.</returns>
        public async Task<bool> RegisterAdmin(string userName, string email, string password)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email
                };

                var createResult = await _userManager.CreateAsync(user, password);
                if (!createResult.Succeeded) return false;

                var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
                return roleResult.Succeeded;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        #endregion

        #region Authentication
        /// <summary>
        /// Logs in a user with the specified email and password.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>True if login was successful; otherwise, false.</returns>
        public async Task<bool> Login(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null && await _userManager.CheckPasswordAsync(user, password))
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Logs out the currently authenticated user.
        /// </summary>
        /// <param name="userId">The ID of the user to log out.</param>
        /// <returns>True if logout was successful; otherwise, false.</returns>
        public async Task<bool> Logout(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                await _signInManager.SignOutAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        #endregion

        #region User Management
        /// <summary>
        /// Retrieves user details by user ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The user details, or null if the user does not exist.</returns>
        public async Task<ApplicationUser> GetUserDetailByUserId(string userId)
        {
            try
            {
                return await _userManager.FindByIdAsync(userId);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Retrieves user details by email.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <returns>The user details, or null if the user does not exist.</returns>
        public async Task<ApplicationUser> GetUserDetailByMail(string email)
        {
            try
            {
                return await _userManager.FindByEmailAsync(email);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        #endregion

        #region Role Management
        /// <summary>
        /// Retrieves the role of a user by their user ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The role of the user, or null if no role is assigned.</returns>
        public async Task<string> GetRoleApplicatonUserByUserIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return null;

                var roles = await _userManager.GetRolesAsync(user);
                return roles.FirstOrDefault(); // Assume a single-role system
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Retrieves the role of a user by their email.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <returns>The role of the user, or an empty string if no role is found.</returns>
        public async Task<string> GetRoleByEmail(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null) return string.Empty;

                var roles = await _userManager.GetRolesAsync(user);
                return roles.FirstOrDefault() ?? string.Empty;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        #endregion
    }
}
