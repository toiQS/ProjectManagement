using Microsoft.AspNetCore.Identity;
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
        private readonly IServiceProvider _serviceProvider;
        public ApplicationUserServices(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager, IServiceProvider serviceProvider)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _serviceProvider = serviceProvider;
        }
        public async Task AddRoleApplication()
        {
            try
            {


                var scope = new[] { "Admin", "User" };
                foreach (var item in scope)
                {
                    if (!await _roleManager.RoleExistsAsync(item))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(item));
                    }
                }
            }
            catch (Exception ex)
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

    }
}
