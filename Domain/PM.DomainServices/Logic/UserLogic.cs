using PM.DomainServices.ILogic;
using PM.DomainServices.Models;
using PM.DomainServices.Models.projects;
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

        public UserLogic(IApplicationUserServices applicationUserServices)
        {
            _applicationUserServices = applicationUserServices;
        }



        #region private method

        #endregion
        #region suport method
        #region Retrieve information about another user by their ID
        /// <summary>
        /// Retrieves detailed information about another user based on their user ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve information for.</param>
        /// <returns>
        /// A service result containing a <see cref="DetailAppUser"/> object if the user exists, 
        /// or an error message if the user does not exist or if the operation fails.
        /// </returns>
        public async Task<ServicesResult<DetailAppUser>> GetInfoOtherUserByUserId(string userId)
        {
            // Validate input
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<DetailAppUser>.Failure("User ID is required.");

            // Attempt to fetch the user by ID or email
            var getUser = await _applicationUserServices.GetAppUserByIdOrEmail(userId);
            if (!getUser.Status || getUser.Data == null)
                return ServicesResult<DetailAppUser>.Failure(getUser.Message);

            // Map retrieved user data to a DetailAppUser object
            var userOther = new DetailAppUser()
            {
                UserId = getUser.Data.Id,
                UserName = getUser.Data.UserName,
                Phone = getUser.Data.Phone,
                Email = getUser.Data.Email,
                FullName = getUser.Data.FullName,
                Avata = getUser.Data.PathImage
            };
            var role = await _applicationUserServices.GetRoleOfUserByEmail(userOther.Email);
            if (role.Status == false) return ServicesResult<DetailAppUser>.Failure(getUser.Message);
            userOther.Role = role.Data;

            // Return success with the user data
            return ServicesResult<DetailAppUser>.Success(userOther, string.Empty);
        }
        #endregion

        #endregion
        #region primary method


        #region Retrieve current user's details
        /// <summary>
        /// Retrieves detailed information about the currently logged-in user.
        /// </summary>
        /// <param name="userCurrentId">The ID of the current user.</param>
        /// <returns>
        /// A service result containing a <see cref="DetailAppUser"/> object if the user exists, 
        /// or an error message if the operation fails.
        /// </returns>
        public async Task<ServicesResult<DetailAppUser>> GetInfoUserCurrent(string userCurrentId)
        {
            // Validate input
            if (string.IsNullOrEmpty(userCurrentId))
                return ServicesResult<DetailAppUser>.Failure("User ID is required.");

            // Attempt to fetch the user by ID or email
            var getUser = await _applicationUserServices.GetAppUserByIdOrEmail(userCurrentId);
            if (!getUser.Status || getUser.Data == null)
                return ServicesResult<DetailAppUser>.Failure(getUser.Message);

            // Update the current user ID
            _userIdCurrent = userCurrentId;

            // Map retrieved user data to a DetailAppUser object
            _userCurrent = new DetailAppUser
            {
                UserId = getUser.Data.Id,
                UserName = getUser.Data.UserName,
                Phone = getUser.Data.Phone,
                Email = getUser.Data.Email,
                FullName = getUser.Data.FullName,
                Avata = getUser.Data.PathImage
            };
            var role = await _applicationUserServices.GetRoleOfUserByEmail(_userCurrent.Email);
            if (role.Status == false) return ServicesResult<DetailAppUser>.Failure(getUser.Message);
            _userCurrent.Role = role.Data;
            return ServicesResult<DetailAppUser>.Success(_userCurrent, string.Empty);
        }
        #endregion


        #endregion

    }
}
