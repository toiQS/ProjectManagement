using PM.DomainServices.ILogic;
using PM.DomainServices.Models;
using PM.DomainServices.Models.users;
using PM.Persistence.IServices;

namespace PM.DomainServices.Logic
{
    public class UserLogic : IUserLogic
    {
        //intialize services
        private IApplicationUserServices _applicationUserServices;
        //intialize logic
        //intialize primary value
        private string _userIdCurrent = string.Empty;
        private DetailAppUser _userCurrent = new DetailAppUser();


        #region private method
        
        #endregion
        #region suport method
        public async Task<ServicesResult<DetailAppUser>> GetInfoOtherUserByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<DetailAppUser>.Failure("User ID is required.");
            // Attempt to fetch the user by ID or email
            var getUser = await _applicationUserServices.GetAppUserByIdOrEmail(userId);
            if (!getUser.Status || getUser.Data == null)
                return ServicesResult<DetailAppUser>.Failure(getUser.Message);

            // Check if user data is available


            // Map user data to ApplicationUser object
            var userOther = new DetailAppUser()
            {
                UserId = getUser.Data.Id,
                UserName = getUser.Data.UserName,
                Phone = getUser.Data.Phone,
                Email = getUser.Data.Email,
                FullName = getUser.Data.FullName,
                Avata = getUser.Data.PathImage
            };

            return ServicesResult<DetailAppUser>.Success(userOther, string.Empty);
        }
        #endregion
        #region primary method

        
        public async Task<ServicesResult<DetailAppUser>> GetInfoUserCurrent(string userCurrentId)
        {
            if (string.IsNullOrEmpty(userCurrentId))
                return ServicesResult<DetailAppUser>.Failure("User ID is required.");
            _userIdCurrent = userCurrentId;
            // Attempt to fetch the user by ID or email
            var getUser = await _applicationUserServices.GetAppUserByIdOrEmail(_userIdCurrent);
            if (!getUser.Status)
                return ServicesResult<DetailAppUser>.Failure(getUser.Message);

            // Check if user data is available
            if (getUser.Data == null)
                return ServicesResult<DetailAppUser>.Success(new DetailAppUser(), "Cannot retrieve user information.");

            // Map user data to ApplicationUser object
            _userCurrent = new DetailAppUser
            {
                UserId = getUser.Data.Id,
                UserName = getUser.Data.UserName,
                Phone = getUser.Data.Phone,
                Email = getUser.Data.Email,
                FullName = getUser.Data.FullName,
                Avata = getUser.Data.PathImage
            };

            return ServicesResult<DetailAppUser>.Success(_userCurrent, string.Empty);
        }
        #endregion
    }
}
