using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.Metrics;
using PM.Domain.DTOs;
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
        public ApplicationUserServices(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager, IServiceProvider serviceProvider, IRepository<ApplicationUser> repository)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _repository = repository;
        }
        public async Task<bool> LoginServices(string email, string password)
        {
            try
            {
                var applicationUser = await _userManager.FindByEmailAsync(email);
                if(applicationUser != null && (await _signInManager.CheckPasswordSignInAsync(applicationUser,password,false) == SignInResult.Success))
                {
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
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
        
        public async Task<bool> RegisterApplicationUser(ApplicationUser user, string password)
        {
            try
            {
                await _userManager.CreateAsync(user);
                await _userManager.AddToRoleAsync(user, "User");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<bool> RegisterApplicationAdmin(ApplicationUser admin, string password)
        {
            try
            {
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
        public async Task<IdentityRole<string>> GetRoleAsync(string roleId)
        {
            try
            {
                return await _roleManager.FindByIdAsync(roleId);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new IdentityRole();
            }

        }
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
    }
}
