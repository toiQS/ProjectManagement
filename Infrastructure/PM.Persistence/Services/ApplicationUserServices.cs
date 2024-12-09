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

        #region Register User
        /// <inheritdoc />
        public async Task<bool> RegisterApplicationUser(string userName, string email, string password)
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
        #endregion

        #region Register Admin
        /// <inheritdoc />
        public async Task<bool> RegisterApplicationAdmin(string userName, string email, string password)
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

        #region Login
        /// <inheritdoc />
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
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        #endregion

        #region Get Role of User
        /// <inheritdoc />
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
        #endregion

        #region Logout
        /// <inheritdoc />
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

        #region Get User by ID
        /// <inheritdoc />
        public async Task<ApplicationUser> GetUser(string userId)
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
        #endregion
        #region Get a user specific  by email
        /// <inheritdoc />
        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                return user;
            }
            catch
            {
                throw;
            }
        }
        #endregion

    }
}
