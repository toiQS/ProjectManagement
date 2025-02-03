using Microsoft.AspNetCore.Identity;
using PM.Domain;
using PM.DomainServices.ILogics;
using PM.DomainServices.Models;
using PM.DomainServices.Models.users;
using PM.DomainServices.UnitOfWorks;

namespace PM.DomainServices.Logics
{
    public class UserLogic : IUserLogic
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserLogic(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<ServicesResult<DetailAppUser>> DetailUserCurrent(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return ServicesResult<DetailAppUser>.Failure("User ID is required");

                var user = await _unitOfWork.ApplicationUserRepository.GetValueByPrimaryKey(userId);
                if (!user.Status)
                    return ServicesResult<DetailAppUser>.Failure(user.Message);

                var role = await _userManager.GetRolesAsync(user.Data);
                if (!role.Any())
                    return ServicesResult<DetailAppUser>.Failure("User does not have a role");

                var detail = new DetailAppUser
                {
                    UserId = userId,
                    FullName = user.Data.FullName,
                    UserName = user.Data.UserName,
                    Avata = user.Data.PathImage,
                    Email = user.Data.Email,
                    Phone = user.Data.Phone,
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

                var user = await _unitOfWork.ApplicationUserRepository.GetValueByPrimaryKey(userId);
                if (!user.Status)
                    return ServicesResult<DetailAppUser>.Failure(user.Message);

                // Kiểm tra giá trị null trước khi cập nhật
                user.Data.FirstName = newInfo.FirstName?.Trim() ?? user.Data.FirstName;
                user.Data.LastName = newInfo.LastName?.Trim() ?? user.Data.LastName;
                user.Data.FullName = $"{user.Data.FirstName} {user.Data.LastName}".Trim();
                user.Data.Phone = newInfo.Phone?.Trim() ?? user.Data.Phone;
                user.Data.Email = newInfo.Email?.Trim() ?? user.Data.Email;
                user.Data.PathImage = newInfo.PathImage?.Trim() ?? user.Data.PathImage;

                var role = await _userManager.GetRolesAsync(user.Data);
                if (!role.Any())
                    return ServicesResult<DetailAppUser>.Failure("User does not have a role");

                var update = await _unitOfWork.ApplicationUserRepository.UpdateAsync(user.Data);
                if (!update.Status)
                    return ServicesResult<DetailAppUser>.Failure(update.Message);

                // Lưu thay đổi vào database
                await _unitOfWork.SaveChangesAsync();

                return ServicesResult<DetailAppUser>.Success(new DetailAppUser()
                {
                    UserId = user.Data.Id,
                    FullName = user.Data.FullName,
                    UserName = user.Data.UserName,
                    Avata = user.Data.PathImage,
                    Email = user.Data.Email,
                    Phone = user.Data.Phone,
                    Role = role.SingleOrDefault()
                }, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailAppUser>.Failure($"Error at logic layer: {ex.Message}");
            }
        }
    }
}
