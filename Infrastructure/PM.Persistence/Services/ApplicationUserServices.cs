using Microsoft.AspNetCore.Identity;
using PM.Domain;
using PM.Persistence.IServices;

namespace PM.Persistence.Services
{
    public class ApplicationUserServices : IApplicationUserServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public ApplicationUserServices(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        #region register user
        public async Task<bool> RegisterApplicationUser(string userName, string email, string password)
        {
            try
            {
                var user = new ApplicationUser()
                {
                    UserName = userName,
                    Email = email,
                };
                if (!(await _userManager.CreateAsync(user, password)).Succeeded) return false;
                
                if(!(await _userManager.AddToRoleAsync(user, "User")).Succeeded) return false;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
        #region register admin
        public async Task<bool> RegisterApplicationAdmin(string userName, string email, string password)
        {
            try
            {
                var user = new ApplicationUser()
                {
                    UserName = userName,
                    Email = email,
                };
                if (!(await _userManager.CreateAsync(user, password)).Succeeded) return false;

                if (!(await _userManager.AddToRoleAsync(user, "Admin")).Succeeded) return false;
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
        #region login
        public async Task<ApplicationUser> LoginServices(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if(user != null && (await _userManager.CheckPasswordAsync(user, password)))
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return user;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ;
            }
        }
        #endregion
        #region get role of application user by user id
        public async Task<string> GetRoleApplicatonUserByUserIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return null;
                var role = await _userManager.GetRolesAsync(user);
                return role.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }

        }
        #endregion

        public async Task Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
