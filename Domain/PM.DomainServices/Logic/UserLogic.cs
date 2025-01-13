using Microsoft.AspNetCore.Identity;
using PM.DomainServices.ILogic;
using PM.DomainServices.Models;
using PM.DomainServices.Models.projects;
using PM.DomainServices.Models.users;
using PM.Persistence.IServices;
using System.Runtime.InteropServices;

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


        #region private method

        #endregion
        #region suport method
        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ServicesResult<DetailAppUser>> GetInfoOtherUserByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<DetailAppUser>.Failure("User ID is required.");
            // Attempt to fetch the user by ID or email
            var getUser = await _applicationUserServices.GetAppUserByIdOrEmail(userId);
            if (!getUser.Status || getUser.Data == null)
                return ServicesResult<DetailAppUser>.Failure(getUser.Message);


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
        #endregion
        #region primary method


        public async Task<ServicesResult<DetailAppUser>> GetInfoUserCurrent(string userCurrentId)
        {
            if (string.IsNullOrEmpty(userCurrentId))
                return ServicesResult<DetailAppUser>.Failure("User ID is required.");
            // Attempt to fetch the user by ID or email
            var getUser = await _applicationUserServices.GetAppUserByIdOrEmail(_userIdCurrent);
            if (!getUser.Status || getUser.Data == null)
                return ServicesResult<DetailAppUser>.Failure(getUser.Message);

          
            _userIdCurrent = userCurrentId;

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
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetListProjectsUserHasJoined()
        {
           var projects = await _projectLogic.GetProjectsUserHasJoined(_userIdCurrent);
           if(projects.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(projects.Message);
            if (projects.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Success(projects.Data, projects.Message);
            return ServicesResult<IEnumerable<IndexProject>>.Success(projects.Data, string.Empty);
        }
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetListProjectsUserHasOwn()
        {
           var projects = await _projectLogic.GetProjectsUserHasOwn(_userIdCurrent);
           if(projects.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(projects.Message);
            if (projects.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Success(projects.Data, projects.Message);
            return ServicesResult<IEnumerable<IndexProject>>.Success(projects.Data, string.Empty);
        }

        #endregion

    }
}
