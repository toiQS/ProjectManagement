using Microsoft.AspNetCore.Identity;
using PM.Domain;
using PM.DomainServices.Models.users;
using PM.DomainServices.Models;
using PM.Persistence.IServices;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Mvc;
using PM.Persistence.Context;

namespace PM.Persistence.Services
{
    /// <summary>
    /// Implements user-related services for authentication, registration, role management, and logout.
    /// </summary>
    public class ApplicationUserServices : IApplicationUserServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public ApplicationUserServices(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<ServicesResult<DetailAppUser>> GetAppUserByIdOrEmail(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return ServicesResult<DetailAppUser>.Failure("");
            try
            {
                var user = new ApplicationUser();
                if (text.Contains("@"))
                {
                    user = await _userManager.FindByEmailAsync(text);
                }
                else user = await _userManager.FindByIdAsync(text);
                var role = await _userManager.GetRolesAsync(user);
                if (user == null) return ServicesResult<DetailAppUser>.Failure("");
                if (role == null) return ServicesResult<DetailAppUser>.Failure("");
                var detailUser = new DetailAppUser()
                {
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Avata = user.PathImage,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = role.FirstOrDefault(),
                    UserId = user.Id
                };
                return ServicesResult<DetailAppUser>.Success(detailUser);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailAppUser>.Failure("");
            }
        }
        
        public async Task<ServicesResult<bool>> UpdateInfoUser(string userId, UpdateAppUser updateAppUser)
        {
            if (!string.IsNullOrEmpty(userId) || updateAppUser == null) return ServicesResult<bool>.Failure("");
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return ServicesResult<bool>.Failure("");
                user.FirstName = updateAppUser.FirstName;
                user.LastName = updateAppUser.LastName;
                user.Email = updateAppUser.Email;
                user.PathImage = updateAppUser.PathImage;
                user.Phone = updateAppUser.Phone;
                user.FullName = $"{user.FirstName} {user.LastName}";
                user.UserName = updateAppUser.UserName;
                _context.Update<ApplicationUser>(user);
                await _context.SaveChangesAsync();
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Failure("");
            }
        }
       
    }
}
