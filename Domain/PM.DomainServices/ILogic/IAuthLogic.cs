using Shared;
using Shared.appUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.DomainServices.ILogic
{
    /// <summary>
    /// Interface defining authentication and user management logic.
    /// </summary>
    public interface IAuthLogic
    {
        #region Authentication

        /// <summary>
        /// Authenticates a user using their login credentials.
        /// </summary>
        /// <param name="loginModel">The login model containing user credentials.</param>
        /// <returns>A service result indicating the success or failure of the login operation.</returns>
        Task<ServicesResult<bool>> Login(LoginModel loginModel);

        /// <summary>
        /// Logs out a user by their user ID.
        /// </summary>
        /// <param name="id">The ID of the user to be logged out.</param>
        /// <returns>A service result indicating the success or failure of the logout operation.</returns>
        Task<ServicesResult<bool>> Logout(string id);

        #endregion

        #region Registration

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="regiserModel">The registration model containing user details.</param>
        /// <returns>A service result indicating the success or failure of the registration operation.</returns>
        Task<ServicesResult<bool>> Register(RegiserModel regiserModel);

        #endregion

        #region User Details

        /// <summary>
        /// Retrieves the detailed information of a user by their user ID.
        /// </summary>
        /// <param name="userId">The ID of the user whose details are being retrieved.</param>
        /// <returns>A service result containing the user's details.</returns>
        Task<ServicesResult<DetailUser>> Detail(string userId);

        #endregion
    }
}
