using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PM.Domain;
using PM.DomainServices.ILogics;
using PM.DomainServices.Models;
using PM.DomainServices.Models.auths;
using PM.DomainServices.Repository;

namespace PM.DomainServices.Logics
{
    public class AuthLogic : IAuthLogic
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthLogic(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ServicesResult<LoginModel>> LoginAsync(LoginModel loginModel)
        {
            if (loginModel is null)
                return ServicesResult<LoginModel>.Failure("User Name and Password may not be null");

            try
            {
                var user = await _userManager.FindByEmailAsync(loginModel.Email);
                if (user is null)
                    return ServicesResult<LoginModel>.Failure("Invalid email or password");

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginModel.Password);
                if (!isPasswordValid)
                    return ServicesResult<LoginModel>.Failure("Invalid email or password");

                await _signInManager.SignInAsync(user, isPersistent: false);
                return ServicesResult<LoginModel>.Success(loginModel, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<LoginModel>.Failure($"An error occurred: {ex.Message}");
            }
            finally
            {
                Dispose();
            }
        }

        public async Task<ServicesResult<RegisterModel>> RegisterAsync(RegisterModel registerModel)
        {
            if (registerModel is null)
                return ServicesResult<RegisterModel>.Failure("Invalid registration data");

            try
            {
                var user = new ApplicationUser
                {
                    UserName = registerModel.Email,
                    Email = registerModel.Email
                };

                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if (!result.Succeeded)
                    return ServicesResult<RegisterModel>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));

                return ServicesResult<RegisterModel>.Success(registerModel, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<RegisterModel>.Failure($"An error occurred: {ex.Message}");
            }
            finally
            {
                Dispose();
            }
        }

        public async Task<ServicesResult<bool>> LogOutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return ServicesResult<bool>.Success(true, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Failure($"An error occurred: {ex.Message}");
            }
        }
        public void Dispose()
        {
            _userManager.Dispose();
        }
    }
}
