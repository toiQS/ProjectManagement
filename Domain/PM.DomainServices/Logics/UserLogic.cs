using Microsoft.AspNetCore.Identity;
using PM.Domain;
using PM.DomainServices.ILogics;
using PM.DomainServices.Models;
using PM.DomainServices.Models.users;
using PM.DomainServices.UnitOfWorks;
using System.Linq;

namespace PM.DomainServices.Logics
{
    public class UserLogic : IUserLogic
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserLogic(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        // Helper method to reduce redundancy
        private async Task<ServicesResult<ApplicationUser>> GetUserByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ServicesResult<ApplicationUser>.Failure("User ID is required");

            var user = await _unitOfWork.ApplicationUserRepository.GetValueByPrimaryKey(userId);
            if (!user.Status)
                return ServicesResult<ApplicationUser>.Failure(user.Message);

            return ServicesResult<ApplicationUser>.Success(user.Data, string.Empty);
        }

        public async Task<ServicesResult<DetailAppUser>> DetailUserCurrent(string userId)
        {
            try
            {
                var userResult = await GetUserByIdAsync(userId);
                if (!userResult.Status)
                    return ServicesResult<DetailAppUser>.Failure(userResult.Message);

                var role = await _userManager.GetRolesAsync(userResult.Data);
                if (!role.Any())
                    return ServicesResult<DetailAppUser>.Failure("User does not have a role");

                var detail = new DetailAppUser
                {
                    UserId = userResult.Data.Id,
                    FullName = userResult.Data.FullName,
                    UserName = userResult.Data.UserName,
                    Avata = userResult.Data.PathImage,
                    Email = userResult.Data.Email,
                    Phone = userResult.Data.Phone,
                    Role = role.SingleOrDefault()
                };

                return ServicesResult<DetailAppUser>.Success(detail, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailAppUser>.Failure($"Error at logic layer: {ex.Message}");
            }
        }

        public async Task<ServicesResult<DetailAppUser>> UpdateInfo(string userId, UpdateAppUser newInfo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId) || newInfo == null)
                    return ServicesResult<DetailAppUser>.Failure("Invalid input");

                var userResult = await GetUserByIdAsync(userId);
                if (!userResult.Status)
                    return ServicesResult<DetailAppUser>.Failure(userResult.Message);

                var user = userResult.Data;

                // Cập nhật các trường thông tin
                user.FirstName = newInfo.FirstName?.Trim() ?? user.FirstName;
                user.LastName = newInfo.LastName?.Trim() ?? user.LastName;
                user.FullName = $"{user.FirstName} {user.LastName}".Trim();
                user.Phone = newInfo.Phone?.Trim() ?? user.Phone;
                user.Email = newInfo.Email?.Trim() ?? user.Email;
                user.PathImage = newInfo.PathImage?.Trim() ?? user.PathImage;

                // Lấy và kiểm tra quyền
                var role = await _userManager.GetRolesAsync(user);
                if (!role.Any())
                    return ServicesResult<DetailAppUser>.Failure("User does not have a role");

                var updateResult = await _unitOfWork.ApplicationUserRepository.UpdateAsync(user);
                if (!updateResult.Status)
                    return ServicesResult<DetailAppUser>.Failure(updateResult.Message);

                await _unitOfWork.SaveChangesAsync();

                var updatedDetail = new DetailAppUser
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Avata = user.PathImage,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = role.SingleOrDefault()
                };

                return ServicesResult<DetailAppUser>.Success(updatedDetail, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailAppUser>.Failure($"Error at logic layer: {ex.Message}");
            }
        }
    }
}
