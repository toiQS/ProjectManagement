using Microsoft.AspNetCore.Identity;
using PM.Domain;
using PM.DomainServices.IServices;
using PM.DomainServices.Models;

namespace PM.DomainServices.Services
{
    /// <summary>
    /// Provides authentication services for user management, login, and logout operations.
    /// </summary>
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthServices(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        #region Login
        /// <summary>
        /// Authenticates a user using their email and password.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>A result containing the authenticated user or an error message.</returns>
        public async Task<ServicesResult<ApplicationUser>> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return ServicesResult<ApplicationUser>.Failure("Email or password cannot be empty.");

            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null && await _userManager.CheckPasswordAsync(user, password))
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return ServicesResult<ApplicationUser>.Success(user);
                }
                return ServicesResult<ApplicationUser>.Failure("Invalid email or password.");
            }
            catch (Exception ex)
            {
                // Log the exception (use a logging framework instead of Console.WriteLine in production)
                Console.WriteLine(ex.Message);
                return ServicesResult<ApplicationUser>.Failure("An error occurred during login.");
            }
        }
        #endregion

        #region Logout
        /// <summary>
        /// Logs out the currently authenticated user.
        /// </summary>
        /// <param name="userId">The ID of the user to log out.</param>
        /// <returns>A result indicating whether the logout was successful.</returns>
        public async Task<ServicesResult<bool>> Logout(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<bool>.Failure("User ID cannot be empty.");

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return ServicesResult<bool>.Failure("User not found.");

                await _signInManager.SignOutAsync();
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                return ServicesResult<bool>.Failure("An error occurred during logout.");
            }
        }
        #endregion

        #region Register
        /// <summary>
        /// Registers a new user with the specified email, username, and password.
        /// </summary>
        /// <param name="email">The email of the new user.</param>
        /// <param name="username">The username of the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <returns>A result indicating whether the registration was successful.</returns>
        public async Task<ServicesResult<bool>> Register(string email, string username, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return ServicesResult<bool>.Failure("Email, username, or password cannot be empty.");

            try
            {
                var user = new ApplicationUser
                {
                    UserName = username,
                    Email = email
                };

                // Create the user
                var createResult = await _userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                    return ServicesResult<bool>.Failure("Failed to create user.");

                // Assign the default role to the user
                var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
                if (!roleResult.Succeeded)
                    return ServicesResult<bool>.Failure("Failed to assign role to the user.");

                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                return ServicesResult<bool>.Failure("An error occurred during registration.");
            }
        }
        #endregion
    }
}
