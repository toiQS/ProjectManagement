using PM.Domain;
using PM.DomainServices.Models;
using PM.DomainServices.Models.positions;
using PM.DomainServices.Models.projects;
using PM.DomainServices.Models.users;
using PM.Persistence.IServices;
using System.Data;

namespace PM.DomainServices.Logic
{
    public class UserLogic
    {
        private readonly IApplicationUserServices _applicationUserServices;
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserServices;
        private readonly IProjectServices _projectServices;
        private readonly IPositionWorkOfMemberServices _positionWorkOfMemberServices;
        private readonly IPositionInProjectServices _positionInProjectServices;

        private readonly IRoleLogic _roleLogic;

        private string _userId = string.Empty;
        private ApplicationUser _user = new ApplicationUser();
        private DetailAppUser _detailAppUser = new DetailAppUser();
        private string _ownerRole = string.Empty;
        private string _leaderRole = string.Empty;
        private string _managerRole = string.Empty;


        // Refactored code for UserLogic with improved readability and added comments.

        #region Check if user exists
        /// <summary>
        /// Checks if a user exists in the system by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to check.</param>
        /// <returns>A service result indicating success or failure, along with a message.</returns>
        public async Task<ServicesResult<string>> CheckUserIsExist(string userId)
        {
            // Validate input
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<string>.Failure("User ID is required.");

            // Attempt to fetch the user by ID or email
            var getUser = await _applicationUserServices.GetAppUserByIdOrEmail(userId);
            if (!getUser.Status)
                return ServicesResult<string>.Failure(getUser.Message);

            // Check if user exists
            if (getUser.Data == null)
                return ServicesResult<string>.Success(string.Empty, $"User does not exist or cannot be found with User ID: {userId}");

            // Store user ID for further operations
            _userId = userId;
            return ServicesResult<string>.Success("Success", string.Empty);
        }
        #endregion

        #region Retrieve another user's details
        /// <summary>
        /// Retrieves detailed information about another user by their ID.
        /// </summary>
        /// <param name="otherUserId">The ID of the other user.</param>
        /// <returns>A service result containing user details or an error message.</returns>
        public async Task<ServicesResult<DetailAppUser>> GetInfoOtherUser(string otherUserId)
        {
            // Validate input
            if (string.IsNullOrEmpty(otherUserId))
                return ServicesResult<DetailAppUser>.Failure("User ID is required.");

            // Attempt to fetch the user by ID or email
            var getUser = await _applicationUserServices.GetAppUserByIdOrEmail(otherUserId);
            if (!getUser.Status)
                return ServicesResult<DetailAppUser>.Failure(getUser.Message);

            if (getUser.Data == null)
                return ServicesResult<DetailAppUser>.Success(new DetailAppUser(), $"User does not exist or cannot be found with User ID: {otherUserId}");

            // Map user data to DetailAppUser object
            var detail = new DetailAppUser
            {
                UserId = getUser.Data.Id,
                UserName = getUser.Data.UserName,
                Phone = getUser.Data.Phone,
                Email = getUser.Data.Email,
                FullName = getUser.Data.FullName,
                Avata = getUser.Data.PathImage
            };

            // Fetch the user's role
            var role = await _applicationUserServices.GetRoleOfUserByEmail(detail.Email);
            if (!role.Status)
                return ServicesResult<DetailAppUser>.Failure(role.Message);

            detail.Role = role.Data ?? "Role not found";
            return ServicesResult<DetailAppUser>.Success(detail, string.Empty);
        }
        #endregion

        #region Retrieve and initialize user details
        /// <summary>
        /// Retrieves detailed information about the currently stored user.
        /// </summary>
        /// <returns>A service result containing user details.</returns>
        private async Task<ServicesResult<ApplicationUser>> GetInfoUser()
        {
            // Attempt to fetch the user by ID or email
            var getUser = await _applicationUserServices.GetAppUserByIdOrEmail(_userId);
            if (!getUser.Status)
                return ServicesResult<ApplicationUser>.Failure(getUser.Message);

            if (getUser.Data == null)
                return ServicesResult<ApplicationUser>.Success(new ApplicationUser(), "Cannot retrieve user information.");

            // Map user data to ApplicationUser object
            _user = new ApplicationUser
            {
                Id = getUser.Data.Id,
                UserName = getUser.Data.UserName,
                FirstName = getUser.Data.FirstName,
                LastName = getUser.Data.LastName,
                Phone = getUser.Data.Phone,
                Email = getUser.Data.Email,
                FullName = getUser.Data.FullName,
                PathImage = getUser.Data.PathImage
            };

            return ServicesResult<ApplicationUser>.Success(getUser.Data, string.Empty);
        }

        /// <summary>
        /// Retrieves the role of the currently stored user.
        /// </summary>
        /// <returns>A service result containing the user's role.</returns>
        private async Task<ServicesResult<string>> GetRole()
        {
            if (string.IsNullOrEmpty(_user.Email))
                return ServicesResult<string>.Failure("User email is null.");

            // Fetch the role of the user by email
            var getRole = await _applicationUserServices.GetRoleOfUserByEmail(_user.Email);
            if (!getRole.Status)
                return ServicesResult<string>.Failure(getRole.Message);

            return ServicesResult<string>.Success(getRole.Data ?? "Role not found", string.Empty);
        }

        /// <summary>
        /// Initializes user information and stores it in the local context.
        /// </summary>
        public void InitializeUserInfo()
        {
            ServicesResult<ApplicationUser> user;
            ServicesResult<string> role;

            // Retry until valid user and role data is obtained
            do
            {
                user = GetInfoUser().GetAwaiter().GetResult();
                role = GetRole().GetAwaiter().GetResult();
            }
            while (user.Data == null || !user.Status || string.IsNullOrEmpty(role.Data) || !role.Status);

            // Populate the DetailAppUser object with user and role data
            _detailAppUser = new DetailAppUser
            {
                FullName = user.Data.FullName,
                UserName = user.Data.UserName,
                UserId = user.Data.Id,
                Avata = user.Data.PathImage,
                Email = user.Data.Email,
                Phone = user.Data.Phone,
                Role = role.Data
            };
        }
        #endregion

        #region Update user information
        /// <summary>
        /// Updates the information of a user.
        /// </summary>
        /// <param name="updateAppUser">The updated user information.</param>
        /// <returns>A service result indicating the success or failure of the update.</returns>
        public async Task<ServicesResult<bool>> UpdateUser(UpdateAppUser updateAppUser)
        {
            if (updateAppUser == null)
                return ServicesResult<bool>.Failure("Update data is required.");

            // Attempt to update the user information
            var resultUpdate = await _applicationUserServices.UpdateInfoUser(_userId, updateAppUser);
            if (!resultUpdate.Status)
                return ServicesResult<bool>.Failure(resultUpdate.Message);

            return ServicesResult<bool>.Success(true, string.Empty);
        }
        #endregion

        

        #region Retrieve projects user has joined
        /// <summary>
        /// Retrieves a list of projects that the currently logged-in user has joined.
        /// </summary>
        /// <returns>A service result containing a list of joined projects.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetListProjectsUserHasJoined()
        {
            var result = new List<IndexProject>();

            // Retrieve all members
            var getMembers = await _roleApplicationUserServices.GetAllAsync();
            if (!getMembers.Status || getMembers.Data == null)
                return ServicesResult<IEnumerable<IndexProject>>.Failure(getMembers.Message ?? "No members found in the database.");

            // Filter projects for the current user
            var projectJoinedResult = getMembers.Data.Where(x => x.ApplicationUserId == _userId).ToList();
            if (!projectJoinedResult.Any())
                return ServicesResult<IEnumerable<IndexProject>>.Success(result, "User has not joined any projects.");

            // Process each project
            foreach (var item in projectJoinedResult)
            {
                // Fetch project details
                var project = await _projectServices.GetValueByPrimaryKeyAsync(item.ProjectId);
                if (project.Data == null || !project.Status || project.Data.IsDeleted)
                    continue;

                // Fetch the project owner details
                var ownerRoleResult = getMembers.Data.FirstOrDefault(x => x.RoleInProjectId == _ownerRole && x.ProjectId == item.ProjectId);
                if (ownerRoleResult == null)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure($"No owner found for project {project.Data.ProjectName}.");

                var owner = await GetInfoOtherUser(ownerRoleResult.ApplicationUserId);
                if (!owner.Status || owner.Data == null)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure($"Cannot retrieve owner info for project {project.Data.ProjectName}.");

                // Add project to the result list
                result.Add(new IndexProject
                {
                    ProjectId = project.Data.Id,
                    ProjectName = project.Data.ProjectName,
                    OwnerName = owner.Data.UserName,
                    OwnerAvata = owner.Data.Avata
                });
            }

            return ServicesResult<IEnumerable<IndexProject>>.Success(result, string.Empty);
        }
        #endregion

        #region Get list of projects user owns
        /// <summary>
        /// Retrieves a list of projects where the currently logged-in user is the owner.
        /// </summary>
        /// <returns>A service result containing a list of projects owned by the user.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectListUserHasOwner()
        {
            var result = new List<IndexProject>();

            // Retrieve all member-role mappings from the role-application-user service
            var getMembers = await _roleApplicationUserServices.GetAllAsync();
            if (!getMembers.Status)
                return ServicesResult<IEnumerable<IndexProject>>.Failure(getMembers.Message);

            if (getMembers.Data == null)
                return ServicesResult<IEnumerable<IndexProject>>.Success(result, "Can't get any member in database");

            // Filter roles where the user is an owner in any project
            var ownerRoles = getMembers.Data
                .Where(x => x.ApplicationUserId == _userId && x.RoleInProjectId == _ownerRole)
                .ToList();

            if (!ownerRoles.Any())
                return ServicesResult<IEnumerable<IndexProject>>.Success(result, "Can't find role user of this project");

            var projects = new List<IndexProject>();

            foreach (var role in ownerRoles)
            {
                // Retrieve project details
                var projectResult = await _projectServices.GetValueByPrimaryKeyAsync(role.ProjectId);
                if (projectResult.Data == null || !projectResult.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure($"{projectResult.Message}, can't get project {role.ProjectId}");
                if (projectResult.Data.IsDeleted)
                    continue;

                // Add the project to the result list
                projects.Add(new IndexProject
                {
                    OwnerAvata = _detailAppUser.Avata,
                    OwnerName = _detailAppUser.UserName,
                    ProjectName = projectResult.Data.ProjectName,
                    ProjectId = projectResult.Data.Id,
                });
            }

            return ServicesResult<IEnumerable<IndexProject>>.Success(projects, string.Empty);
        }
        #endregion

        #region list roles and position work of user in all projects
        public async Task<ServicesResult<IEnumerable<UserOverView>>> GetListOverViewUser()
        {
            var projectJoined = await GetListProjectsUserHasJoined();
            if (projectJoined.Data == null) return ServicesResult<IEnumerable<UserOverView>>.Success(new List<UserOverView>(), projectJoined.Message);
            if(projectJoined.Status == false) return ServicesResult<IEnumerable<UserOverView>>.Failure( projectJoined.Message);
            foreach (var item in projectJoined.Data)
            {
                var getRolesInProject = await _roleApplicationUserServices.GetAllAsync();
                if (getRolesInProject.Status == false) return ServicesResult<IEnumerable<UserOverView>>.Failure(getRolesInProject.Message);
                if (getRolesInProject.Data == null) return ServicesResult<IEnumerable<UserOverView>>.Success(new List<UserOverView>(), "can't get any role in databse");
                var userRole = getRolesInProject.Data.FirstOrDefault(x => x.ApplicationUserId == _userId && x.ProjectId == item.ProjectId);
                if (userRole == null) return ServicesResult<IEnumerable<UserOverView>>.Failure("user didn't joined this project {item.ProjectName}, but can get id of this project");

                var infoRoleUser = await _roleInProjectServices.GetValueByPrimaryKeyAsync(userRole.RoleInProjectId);
                if(infoRoleUser.Status == false) return ServicesResult<IEnumerable<UserOverView>>.Failure("can't get any role project in databse");
                if (infoRoleUser.Data == null) return ServicesResult<IEnumerable<UserOverView>>.Success(new List<UserOverView>(), "can't get this role of user");\
                
                // Retrieve position work assignments
                var positionWorks = await _positionWorkOfMemberServices.GetAllAsync();
                if (!positionWorks.Status)
                    return ServicesResult<IEnumerable<UserOverView>>.Failure("Unable to retrieve position work assignments from the database.");
                if (positionWorks.Data == null)
                    return ServicesResult<IEnumerable<UserOverView>>.Failure("No position work assignments found for members.");

                var userPositionWork = positionWorks.Data.FirstOrDefault(x => x.RoleApplicationUserInProjectId == userRole.Id);
                if (userPositionWork == null)
                    return ServicesResult<IEnumerable<UserOverView>>.Failure($"User {_detailAppUser.UserName} does not have a position assigned in this project.");
                var position = await GetPositonsInProject(item.ProjectId);
               
                
            }
        }
        #endregion
        private async Task<ServicesResult<IEnumerable<IndexPosition>>> GetPositonsInProject(string projectId)
        {
            if (projectId == null) return ServicesResult<IEnumerable<IndexPosition>>.Failure("");
            var positions = await _positionInProjectServices.GetAllAsync();
            if (positions.Data == null) return ServicesResult<IEnumerable<IndexPosition>>.Success(new List<IndexPosition>(),"");
            if (!positions.Status) return ServicesResult<IEnumerable<IndexPosition>>.Failure(positions.Message);
            var posionProject = positions.Data.Where(x => x.ProjectId == projectId);
            if(posionProject == null) return ServicesResult<IEnumerable<IndexPosition>>.Success(new List<IndexPosition>(),"can't get any position in this project");
            var data = posionProject.Select(x => new IndexPosition()
            {
                PositionName = x.PositionName,
                PositionId = x.Id,
                PrositionDescription = x.PositionDescription,
            });
            return ServicesResult<IEnumerable<IndexPosition>>.Success(data  , string.Empty);
        }
    }
}
