using PM.DomainServices.Models;
using PM.DomainServices.Models.users;

namespace PM.DomainServices.ILogic
{
    /// <summary>
    /// Interface defining project-related operations and management logic.
    /// </summary>
    public interface IProjectLogic
    {
        #region check and get user info
        /// <summary>
        /// Check user is existed in database
        /// If status true or data not null then access for next action
        /// </summary>
        /// <param name="userId">The Id of the user</param>
        /// <returns>A service result contain info of user</returns>
        public Task<ServicesResult<DetailAppUser>> CheckAndGetUser(string userId);
        #endregion
    }
}
