using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.Metrics;
using PM.Domain;
using PM.Persistence.Context;
using PM.Persistence.IServices;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.Services
{
    public class ApplicationUserServices : IApplicationUserServices
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public readonly IRepository<ApplicationUser> _repository;
        public readonly IUserRoleStore<ApplicationUser> _userRoleStore;
        #region constructor
        public ApplicationUserServices(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager, IServiceProvider serviceProvider, IRepository<ApplicationUser> repository, IUserRoleStore<ApplicationUser> userRoleStore)
        {
            _context = context;
            _userRoleStore = userRoleStore; 
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _repository = repository;
        }
        #endregion
        #region login
        public async Task<ApplicationUser> LoginServices(string email, string password)
        {
            try
            {
                var applicationUser = await _userManager.FindByEmailAsync(email);
                if(applicationUser != null && (await _signInManager.CheckPasswordSignInAsync(applicationUser,password,false) == SignInResult.Success))
                {
                    return applicationUser;
                }
                return null;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion
        #region logout
        public async Task Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion
        #region register user
        public async Task<bool> RegisterApplicationUser(string userName, string email, string password)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,

                };
                await _userManager.CreateAsync(user,password);
                await _userManager.AddToRoleAsync(user, "User");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion
        #region register admin
        public async Task<bool> RegisterApplicationAdmin(string userName, string email, string password)
        {
            try
            {
                var admin = new ApplicationUser()
                {
                    UserName =userName,
                    Email =email,
                };
                await _userManager.CreateAsync(admin);
                await _userManager.AddToRoleAsync(admin, "Admin");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion
        #region get user by user id
        public async Task<ApplicationUser> GetApplicationUserAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) return new ApplicationUser();
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return new ApplicationUser();
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }

        }
        #endregion
        #region get all user
        public async Task<IEnumerable<ApplicationUser>> GetAllUser()
        {
            try
            {
                return await _context.ApplicationUser.ToListAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<ApplicationUser>();
            }
        }
        #endregion
        
    }
}
