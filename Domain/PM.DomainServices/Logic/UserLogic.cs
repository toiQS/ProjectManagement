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
        private readonly IMemberLogic _memberLogic;
        private readonly IProjectLogic _projectLogic;
        //intialize primary value
        private string _userIdCurrent = string.Empty;
        private DetailAppUser _userCurrent = new DetailAppUser();

        public UserLogic(IApplicationUserServices applicationUserServices, IMemberLogic memberLogic, IProjectLogic projectLogic, string userIdCurrent, DetailAppUser userCurrent)
        {
            _applicationUserServices = applicationUserServices;
            _memberLogic = memberLogic;
            _projectLogic = projectLogic;
            _userIdCurrent = userIdCurrent;
            _userCurrent = userCurrent;
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

            return ServicesResult<DetailAppUser>.Success(_userCurrent, string.Empty);
        }
        #endregion

        #region Retrieve projects the current user has joined
        /// <summary>
        /// Retrieves a list of projects that the currently logged-in user has joined.
        /// </summary>
        /// <returns>
        /// A service result containing a list of <see cref="IndexProject"/> objects if available, 
        /// or an error message if the operation fails.
        /// </returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetListProjectsUserHasJoined()
        {
            // Fetch projects the user has joined
            var projects = await _projectLogic.GetProjectsUserHasJoined(_userIdCurrent);
            if (!projects.Status)
                return ServicesResult<IEnumerable<IndexProject>>.Failure(projects.Message);

            return ServicesResult<IEnumerable<IndexProject>>.Success(projects.Data ?? new List<IndexProject>(), projects.Message ?? string.Empty);
        }
        #endregion

        #region Retrieve projects the current user owns
        /// <summary>
        /// Retrieves a list of projects where the currently logged-in user is the owner.
        /// </summary>
        /// <returns>
        /// A service result containing a list of <see cref="IndexProject"/> objects if available, 
        /// or an error message if the operation fails.
        /// </returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetListProjectsUserHasOwn()
        {
            // Fetch projects the user owns
            var projects = await _projectLogic.GetProjectsUserHasOwn(_userIdCurrent);
            if (!projects.Status)
                return ServicesResult<IEnumerable<IndexProject>>.Failure(projects.Message);

            return ServicesResult<IEnumerable<IndexProject>>.Success(projects.Data ?? new List<IndexProject>(), projects.Message ?? string.Empty);
        }
        #endregion


        #endregion

    }
}
