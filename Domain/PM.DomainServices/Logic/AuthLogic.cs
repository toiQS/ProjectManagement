using PM.DomainServices.ILogic;
using PM.Persistence.IServices;
using Shared;
using Shared.appUser;

namespace PM.DomainServices.Logic
{
    /// <summary>
    /// Implements authentication and user management logic.
    /// </summary>
    public class AuthLogic : IAuthLogic
    {
        private readonly IApplicationUserServices _applicationUserServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthLogic"/> class.
        /// </summary>
        /// <param name="applicationUserServices">Service for user-related operations.</param>
        public AuthLogic(IApplicationUserServices applicationUserServices)
        {
            _applicationUserServices = applicationUserServices;
        }

        #region Login
        /// <inheritdoc />
        public async Task<ServicesResult<bool>> Login(LoginModel loginModel)
        {
            if (loginModel == null)
                return ServicesResult<bool>.Failure("Login model is null.");

            var user = await _applicationUserServices.Login(loginModel.Email, loginModel.Password);
            if (!user)
                return ServicesResult<bool>.Failure("Invalid email or password.");

            return ServicesResult<bool>.Success(true);
        }
        #endregion

        #region Register
        /// <inheritdoc />
        public async Task<ServicesResult<bool>> Register(RegiserModel regiserModel)
        {
            if (regiserModel == null)
                return ServicesResult<bool>.Failure("Register model is null.");

            var registrationSuccess = await _applicationUserServices.RegisterUser(
                regiserModel.UserName,
                regiserModel.Email,
                regiserModel.Password);

            if (!registrationSuccess)
                return ServicesResult<bool>.Failure("Registration failed. Ensure the details are correct.");

            return ServicesResult<bool>.Success(true);
        }
        #endregion

        #region Get User Details
        /// <inheritdoc />
        public async Task<ServicesResult<DetailUser>> Detail(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<DetailUser>.Failure("User ID is null or empty.");

            var user = await _applicationUserServices.GetUserDetailByUserId(userId);
            if (user == null)
                return ServicesResult<DetailUser>.Failure("User not found.");

            var role = await _applicationUserServices.GetRoleByEmail(user.Email);
            var result = new DetailUser
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Avata = user.PathImage,
                Email = user.Email,
                Phone = user.Phone,
                Role = role
            };

            return ServicesResult<DetailUser>.Success(result);
        }
        #endregion

        #region Logout
        /// <inheritdoc />
        public async Task<ServicesResult<bool>> Logout(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<bool>.Failure("User ID is null or empty.");

            var logoutSuccess = await _applicationUserServices.Logout(userId);
            if (!logoutSuccess)
                return ServicesResult<bool>.Failure("Logout failed.");

            return ServicesResult<bool>.Success(true);
        }
        #endregion

        public async Task<ServicesResult<string>> GetRoleByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) ServicesResult<string>.Failure("");
            var role = await _applicationUserServices.GetRoleByEmail(email);
            if (role == null) return ServicesResult<string>.Failure("");
            return ServicesResult<string>.Success(role);
        }
    }
}
