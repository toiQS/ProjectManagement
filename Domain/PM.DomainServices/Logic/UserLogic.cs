using Microsoft.AspNetCore.Identity.UI.Services;
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
        // Private readonly fields for services used in the logic
        private readonly IApplicationUserServices _applicationUserServices;
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserServices;
        private readonly IProjectServices _projectServices;
        private readonly IPositionWorkOfMemberServices _positionWorkOfMemberServices;
        private readonly IPositionInProjectServices _positionInProjectServices;
        private readonly IRoleLogic _roleLogic; // Logic related to roles

        // Private variables to store user and role information
        private string _userId = string.Empty; // User ID
        private ApplicationUser _user = new ApplicationUser(); // User data
        private DetailAppUser _detailAppUser = new DetailAppUser(); // Detailed user data
        private string _ownerRole = string.Empty; // ID for the 'Owner' role
        private string _leaderRole = string.Empty; // ID for the 'Leader' role
        private string _managerRole = string.Empty; // ID for the 'Manager' role

        // Constructor for UserLogic that initializes the services and sets up the necessary user-related data
        public UserLogic(
            IApplicationUserServices applicationUserServices,
            IRoleApplicationUserInProjectServices roleApplicationUserInProjectServices,
            IProjectServices projectServices,
            IPositionInProjectServices positionInProjectServices,
            IPositionWorkOfMemberServices positionWorkOfMemberServices)
        {
            // Assign dependencies (services) to readonly fields
            _applicationUserServices = applicationUserServices;
            _roleApplicationUserServices = roleApplicationUserInProjectServices;
            _projectServices = projectServices;
            _positionInProjectServices = positionInProjectServices;
            _positionWorkOfMemberServices = positionWorkOfMemberServices;

            // Initialize user information and roles
            InitializeUserInfo();
            InitializeRole();
        }


        // Method to initialize role data by fetching the owner, leader, and manager roles
        private void InitializeRole()
        {
            // Variables to store the results of fetching roles
            var ownerRole = new ServicesResult<RoleInProject>();
            var leaderRole = new ServicesResult<RoleInProject>();
            var managerRole = new ServicesResult<RoleInProject>();

            // Fetch the roles until all roles are successfully retrieved
            do
            {
                ownerRole = _roleLogic.GetOwnerRole().GetAwaiter().GetResult();
                leaderRole = _roleLogic.GetLeaderRole().GetAwaiter().GetResult();
                managerRole = _roleLogic.GetManagerRole().GetAwaiter().GetResult();
            }
            while (
                ownerRole.Data == null || ownerRole.Status == false ||
                leaderRole.Status == false || leaderRole.Data == null ||
                managerRole.Data == null || managerRole.Status == false
            );

            // Assign the role IDs after successfully fetching the roles
            _ownerRole = ownerRole.Data.Id;
            _leaderRole = leaderRole.Data.Id;
            _managerRole = managerRole.Data.Id;
        }


        #region Check if user exists
        /// <summary>
        /// Checks if a user exists in the system by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to check.</param>
        /// <returns>A service result indicating success or failure, along with a message.</returns>
        public async Task<ServicesResult<string>> CheckUserIsExist(string userId)
        {
            // Validate input to ensure userId is not null or empty
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<string>.Failure("User ID is required.");

            // Attempt to fetch the user by ID or email
            var getUser = await _applicationUserServices.GetAppUserByIdOrEmail(userId);
            if (!getUser.Status)
                return ServicesResult<string>.Failure(getUser.Message);

            // Check if user exists in the system
            if (getUser.Data == null)
                return ServicesResult<string>.Success(string.Empty, $"User does not exist or cannot be found with User ID: {userId}");

            // Store the user ID for further operations
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
            // Validate input to ensure otherUserId is not null or empty
            if (string.IsNullOrEmpty(otherUserId))
                return ServicesResult<DetailAppUser>.Failure("User ID is required.");

            // Attempt to fetch the user by ID or email
            var getUser = await _applicationUserServices.GetAppUserByIdOrEmail(otherUserId);
            if (!getUser.Status)
                return ServicesResult<DetailAppUser>.Failure(getUser.Message);

            // Check if user data is available
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

            // Assign the role to the user detail
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

            // Check if user data is available
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
            // Ensure the user's email is available before fetching their role
            if (string.IsNullOrEmpty(_user.Email))
                return ServicesResult<string>.Failure("User email is null.");

            // Fetch the user's role by email
            var getRole = await _applicationUserServices.GetRoleOfUserByEmail(_user.Email);
            if (!getRole.Status)
                return ServicesResult<string>.Failure(getRole.Message);

            // Return the role, or "Role not found" if not available
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
            var result = new List<IndexProject>(); // Initialize the result list

            // Retrieve all members of projects
            var getMembers = await _roleApplicationUserServices.GetAllAsync();
            if (!getMembers.Status || getMembers.Data == null)
                return ServicesResult<IEnumerable<IndexProject>>.Failure(getMembers.Message ?? "No members found in the database.");

            // Filter the members to find projects the current user has joined
            var projectJoinedResult = getMembers.Data.Where(x => x.ApplicationUserId == _userId).ToList();
            if (!projectJoinedResult.Any())
                return ServicesResult<IEnumerable<IndexProject>>.Success(result, "User has not joined any projects.");

            // Process each project the user has joined
            foreach (var item in projectJoinedResult)
            {
                // Fetch project details by project ID
                var project = await _projectServices.GetValueByPrimaryKeyAsync(item.ProjectId);
                if (project.Data == null || !project.Status || project.Data.IsDeleted)
                    continue; // Skip if the project is not found, status is false, or it is deleted

                // Fetch the owner of the project by checking the project role
                var ownerRoleResult = getMembers.Data.FirstOrDefault(x => x.RoleInProjectId == _ownerRole && x.ProjectId == item.ProjectId);
                if (ownerRoleResult == null)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure($"No owner found for project {project.Data.ProjectName}.");

                // Fetch detailed information about the owner of the project
                var owner = await GetInfoOtherUser(ownerRoleResult.ApplicationUserId);
                if (!owner.Status || owner.Data == null)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure($"Cannot retrieve owner info for project {project.Data.ProjectName}.");

                // Add the project to the result list along with owner information
                result.Add(new IndexProject
                {
                    ProjectId = project.Data.Id,
                    ProjectName = project.Data.ProjectName,
                    OwnerName = owner.Data.UserName,
                    OwnerAvata = owner.Data.Avata
                });
            }

            // Return the final list of projects the user has joined
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
            var result = new List<IndexProject>(); // Initialize the result list for projects owned by the user

            // Retrieve all member-role mappings from the role-application-user service
            var getMembers = await _roleApplicationUserServices.GetAllAsync();
            if (!getMembers.Status)
                return ServicesResult<IEnumerable<IndexProject>>.Failure(getMembers.Message); // Return failure if retrieval fails

            if (getMembers.Data == null)
                return ServicesResult<IEnumerable<IndexProject>>.Success(result, "Can't get any member in database"); // Return success with empty result if no data

            // Filter roles where the user is an owner in any project
            var ownerRoles = getMembers.Data
                .Where(x => x.ApplicationUserId == _userId && x.RoleInProjectId == _ownerRole)
                .ToList();

            if (!ownerRoles.Any())
                return ServicesResult<IEnumerable<IndexProject>>.Success(result, "Can't find role user of this project"); // Return success with empty result if no owner role is found

            var projects = new List<IndexProject>(); // List to hold projects owned by the user

            // Loop through each role where the user is the owner and fetch project details
            foreach (var role in ownerRoles)
            {
                // Retrieve project details using the project ID
                var projectResult = await _projectServices.GetValueByPrimaryKeyAsync(role.ProjectId);
                if (projectResult.Data == null || !projectResult.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure($"{projectResult.Message}, can't get project {role.ProjectId}"); // Return failure if project is not found or retrieval fails

                // Skip deleted projects
                if (projectResult.Data.IsDeleted)
                    continue;

                // Add the project to the result list
                projects.Add(new IndexProject
                {
                    OwnerAvata = _detailAppUser.Avata, // Owner avatar from the current user details
                    OwnerName = _detailAppUser.UserName, // Owner name from the current user details
                    ProjectName = projectResult.Data.ProjectName, // Project name
                    ProjectId = projectResult.Data.Id, // Project ID
                });
            }

            // Return the list of projects the user owns
            return ServicesResult<IEnumerable<IndexProject>>.Success(projects, string.Empty);
        }
        #endregion


        #region List roles and position work of user in all projects
        /// <summary>
        /// Retrieves an overview of the user's roles and position work in all the projects they have joined.
        /// </summary>
        /// <returns>A service result containing the list of user overviews, including project roles and position work assignments.</returns>
        public async Task<ServicesResult<IEnumerable<UserOverView>>> GetListOverViewUser()
        {
            // Retrieve the list of projects the user has joined
            var projectJoined = await GetListProjectsUserHasJoined();

            // If the user is not part of any project, return an empty list with the appropriate message
            if (projectJoined.Data == null)
                return ServicesResult<IEnumerable<UserOverView>>.Success(new List<UserOverView>(), projectJoined.Message);

            // If there is a failure in retrieving the list of projects, return failure
            if (projectJoined.Status == false)
                return ServicesResult<IEnumerable<UserOverView>>.Failure(projectJoined.Message);

            // Initialize the result list to hold the user overview for each project
            var result = new List<UserOverView>();

            // Loop through each project the user has joined
            foreach (var item in projectJoined.Data)
            {
                // Retrieve all role assignments for users in the project
                var getRolesInProject = await _roleApplicationUserServices.GetAllAsync();

                // If retrieval fails, return a failure result
                if (getRolesInProject.Status == false)
                    return ServicesResult<IEnumerable<UserOverView>>.Failure(getRolesInProject.Message);

                // If no roles are found in the database, return an empty list with an appropriate message
                if (getRolesInProject.Data == null)
                    return ServicesResult<IEnumerable<UserOverView>>.Success(new List<UserOverView>(), "Can't get any role in the database");

                // Find the role of the current user in the project
                var userRole = getRolesInProject.Data.FirstOrDefault(x => x.ApplicationUserId == _userId && x.ProjectId == item.ProjectId);

                // If the user is not assigned a role in this project, return failure
                if (userRole == null)
                    return ServicesResult<IEnumerable<UserOverView>>.Failure($"User didn't join project {item.ProjectName}, but can get id of this project");

                // Retrieve role details for the user in the project
                var infoRoleUser = await _roleLogic.GetInfoRole(userRole.RoleInProjectId);

                // If role details can't be retrieved, return failure
                if (infoRoleUser.Status == false)
                    return ServicesResult<IEnumerable<UserOverView>>.Failure("Can't get any role project in the database");

                // If no role information is found, return an empty list with an appropriate message
                if (infoRoleUser.Data == null)
                    return ServicesResult<IEnumerable<UserOverView>>.Success(new List<UserOverView>(), "Can't get this role of user");

                // Retrieve all position work assignments for members
                var positionWorks = await _positionWorkOfMemberServices.GetAllAsync();

                // If position work assignments cannot be retrieved, return failure
                if (!positionWorks.Status)
                    return ServicesResult<IEnumerable<UserOverView>>.Failure("Unable to retrieve position work assignments from the database.");

                // If no position work assignments are found, return failure
                if (positionWorks.Data == null)
                    return ServicesResult<IEnumerable<UserOverView>>.Failure("No position work assignments found for members.");

                // Find the user's assigned position in the project
                var userPositionWork = positionWorks.Data.FirstOrDefault(x => x.RoleApplicationUserInProjectId == userRole.Id);

                // If the user doesn't have a position in the project, return failure
                if (userPositionWork == null)
                    return ServicesResult<IEnumerable<UserOverView>>.Failure($"User {_detailAppUser.UserName} does not have a position assigned in this project.");

                // Retrieve the position details
                var position = await _positionInProjectServices.GetValueByPrimaryKeyAsync(userPositionWork.PostitionInProjectId);

                // If position details can't be retrieved, return failure
                if (position.Status == false)
                    return ServicesResult<IEnumerable<UserOverView>>.Failure(position.Message);

                // If no position information is found, return failure
                if (position.Data == null)
                    return ServicesResult<IEnumerable<UserOverView>>.Failure(position.Message);

                // Create the UserOverView object with the retrieved details
                var data = new UserOverView()
                {
                    PositionWorkName = position.Data.PositionName, // Position name in the project
                    ProjectName = item.ProjectName, // Name of the project
                    RoleName = infoRoleUser.Data.RoleName, // Role name in the project
                };

                // Add the UserOverView object to the result list
                result.Add(data);
            }

            // Return the list of user overviews with their roles and position work in each project
            return ServicesResult<IEnumerable<UserOverView>>.Success(result, string.Empty);
        }
        #endregion

    }
}
