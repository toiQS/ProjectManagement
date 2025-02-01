using Microsoft.AspNetCore.Identity;
using PM.Domain;
using PM.DomainServices.ILogics;
using PM.DomainServices.Models;
using PM.DomainServices.Models.users;
using PM.DomainServices.Repository;
using PM.Persistence.Context;

namespace PM.DomainServices.Logics
{
    public class UserLogic : IUserLogic
    {
        private readonly IRepository<ApplicationUser> _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserLogic(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _repository = new Repository<ApplicationUser>(context);
            _userManager = userManager;
        }

        public async Task<ServicesResult<DetailAppUser>> DetailUserCurrent(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return ServicesResult<DetailAppUser>.Failure("User ID is required");

                var responseUser = await _repository.GetValueByPrimaryKey(userId);
                if (!responseUser.Status)
                    return ServicesResult<DetailAppUser>.Failure(responseUser.Message);

                var role = await _userManager.GetRolesAsync(responseUser.Data);
                var response = new DetailAppUser()
                {
                    Email = responseUser.Data.Email,
                    FullName = responseUser.Data.FullName,
                    UserName = responseUser.Data.UserName,
                    Avata = responseUser.Data.PathImage,
                    Phone = responseUser.Data.Phone,
                    UserId = userId,
                    Role = role.FirstOrDefault()
                };
                return ServicesResult<DetailAppUser>.Success(response, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailAppUser>.Failure("error at logic layer:"+ ex.Source);
            }
            finally
            {
                Dispose();
            }
        }

        public async Task<ServicesResult<DetailAppUser>> UpdateInfo(string userId, DetailAppUser newInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || newInfo == null)
                    return ServicesResult<DetailAppUser>.Failure("Invalid input");

                var currentUser = await _repository.GetValueByPrimaryKey(userId);
                if (!currentUser.Status)
                    return ServicesResult<DetailAppUser>.Failure(currentUser.Message);

                var arr = newInfo.FullName.Split(" ");
                if (arr.Length == 1)
                {
                    currentUser.Data.LastName = arr[0];
                }
                else if (arr.Length >= 2)
                {
                    currentUser.Data.FirstName = arr[0];
                    currentUser.Data.LastName = arr[arr.Length - 1];
                }

                currentUser.Data.Email = newInfo.Email;
                currentUser.Data.Phone = newInfo.Phone;
                currentUser.Data.PathImage = newInfo.Avata;
                currentUser.Data.UserName = newInfo.UserName;

                var update = await _repository.UpdateAsync(currentUser.Data);
                if (!update.Status)
                    return ServicesResult<DetailAppUser>.Failure(update.Message);
                return ServicesResult<DetailAppUser>.Success(newInfo, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailAppUser>.Failure("error at logic layer:" + ex.Source);
            }
            finally
            {
                Dispose();
            }
        }
        public void Dispose()
        {
            _userManager.Dispose();
        }
    }
}
