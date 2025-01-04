using Microsoft.AspNetCore.Identity;
using PM.Domain;
using PM.DomainServices.IServices;
using PM.DomainServices.Models;

namespace PM.DomainServices.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AuthServices(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<ServicesResult<ApplicationUser>> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return ServicesResult<ApplicationUser>.Failure("");
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null && await _userManager.CheckPasswordAsync(user, password))
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return ServicesResult<ApplicationUser>.Success(user);
                }
                return ServicesResult<ApplicationUser>.Failure("");
            }
            catch (Exception ex)
            {
                return ServicesResult<ApplicationUser>.Failure("");
            }
            
        }
        #endregion
        public async Task<ServicesResult<bool>> Logout(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return ServicesResult<bool>.Failure("");
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return ServicesResult<bool>.Failure("");
                await _signInManager.SignOutAsync();
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Failure("");
            }
        }
        public async Task<ServicesResult<bool>> Register(string email, string username, string password)
        {
            if (!string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return ServicesResult<bool>.Failure("");
            
                try
                {
                    var user = new ApplicationUser

                    {
                        UserName = username,
                        Email = email
                    };

                    var createResult = await _userManager.CreateAsync(user, password);
                    if (!createResult.Succeeded) return ServicesResult<bool>.Failure("");

                    var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
                    return ServicesResult<bool>.Success(true);
                }
                catch (Exception ex)
                {
                    // Log the exception (example: use a logging framework)
                    Console.WriteLine(ex.Message);
                    return ServicesResult<bool>.Failure("");
                }
            
        }
    }
}
