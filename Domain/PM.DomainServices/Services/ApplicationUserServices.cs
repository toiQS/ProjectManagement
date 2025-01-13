using Microsoft.AspNetCore.Identity;
using PM.Domain;
using PM.DomainServices.Models.users;
using PM.DomainServices.Models;
using PM.Persistence.IServices;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PM.Persistence.Context;
using System;
using System.Threading.Tasks;

namespace PM.Persistence.Services
{
    /// <summary>
    /// Implements user-related services for authentication, registration, role management, and logout.
    /// </summary>
    public class ApplicationUserServices : IApplicationUserServices
    {
        #region Private Fields

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationUserServices"/> class.
        /// </summary>
        /// <param name="userManager">The UserManager for managing application users.</param>
        /// <param name="context">The database context.</param>
        public ApplicationUserServices(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        #endregion

        #region Get User by ID or Email

        /// <summary>
        /// Retrieves application user details by their ID or email.
        /// </summary>
        /// <param name="text">The user ID or email.</param>
        /// <returns>A <see cref="ServicesResult{DetailAppUser}"/> containing the user details or an error message.</returns>
        public async Task<ServicesResult<ApplicationUser>> GetAppUserByIdOrEmail(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return ServicesResult<ApplicationUser>.Failure("Input text cannot be null or empty.");

            try
            {
                ApplicationUser user = new ApplicationUser();

                // Determine if the input is an email or ID
                if (text.Contains("@"))
                {
                    user = await _userManager.FindByEmailAsync(text);
                }
                else
                {
                    user = await _userManager.FindByIdAsync(text);
                }

                if (user == null)
                    return ServicesResult<ApplicationUser>.Success(new ApplicationUser(),"User not found.");

                return ServicesResult<ApplicationUser>.Success(user,string.Empty);
            }
            catch (Exception ex)
            {
                // Log exception (placeholder for a logging framework)
                Console.WriteLine($"Error: {ex.Message}");
                return ServicesResult<ApplicationUser>.Failure("An error occurred while retrieving user details.");
            }
        }

        #endregion

        #region Update User Info

        /// <summary>
        /// Updates the information of an application user.
        /// </summary>
        /// <param name="userId">The user ID of the user to update.</param>
        /// <param name="updateAppUser">The new user information.</param>
        /// <returns>A <see cref="ServicesResult{bool}"/> indicating success or failure.</returns>
        public async Task<ServicesResult<bool>> UpdateInfoUser(string userId, UpdateAppUser updateAppUser)
        {
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<bool>.Failure("User ID cannot be null or empty.");

            if (updateAppUser == null)
                return ServicesResult<bool>.Failure("Update data cannot be null.");

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return ServicesResult<bool>.Failure("User not found.");

                // Update user fields
                user.FirstName = updateAppUser.FirstName;
                user.LastName = updateAppUser.LastName;
                user.Email = updateAppUser.Email;
                user.PathImage = updateAppUser.PathImage;
                user.Phone = updateAppUser.Phone;
                user.FullName = $"{user.FirstName} {user.LastName}";
                user.UserName = updateAppUser.UserName;

                _context.Update(user);
                await _context.SaveChangesAsync();

                return ServicesResult<bool>.Success(true,string.Empty);
            }
            catch (Exception ex)
            {
                // Log exception (placeholder for a logging framework)
                Console.WriteLine($"Error: {ex.Message}");
                return ServicesResult<bool>.Failure("An error occurred while updating user information.");
            }
        }

        #endregion

        public async Task<ServicesResult<string>> GetRoleOfUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return ServicesResult<string>.Failure("");
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null) return ServicesResult<string>.Failure("");
                var role = await _userManager.GetRolesAsync(user);
                if (role == null) return ServicesResult<string>.Failure("");
                return ServicesResult<string>.Success(role.First(), string.Empty);
            }
            catch (Exception ex)
            {
                // Log exception (placeholder for a logging framework)
                Console.WriteLine($"Error: {ex.Message}");
                return ServicesResult<string>.Failure("An error occurred while updating user information.");
            }
        }

    }
}
