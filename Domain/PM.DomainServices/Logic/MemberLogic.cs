using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using PM.Domain;
using PM.DomainServices.Models;
using PM.DomainServices.Models.members;
using PM.DomainServices.Models.tasks;
using PM.DomainServices.Models.users;
using PM.Persistence.IServices;
using Shared.member;

namespace PM.DomainServices.Logic
{
    public class MemberLogic
    {
        private readonly IApplicationUserServices _applicationUserServices;
        private readonly IMemberInTaskServices _memberInTaskServices;
        private readonly IPositionInProjectServices _positionInProjectServices;
        private readonly IPositionWorkOfMemberServices _positionWorkOfMemberServices;
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserServices;
        private readonly IProjectServices _projectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        private readonly ITaskServices _taskServices;
        private readonly IStatusServices _statusServices;
        private string _project = string.Empty;
        private DetailAppUser _user = new DetailAppUser();
        private string _ownerRole = string.Empty;
        private List<Status> _statuses = new List<Status>();

        public MemberLogic(IApplicationUserServices applicationUserServices, IMemberInTaskServices memberInTaskServices, IPositionInProjectServices positionInProjectServices, IPositionWorkOfMemberServices positionWorkOfMemberServices, IRoleApplicationUserInProjectServices roleApplicationUserServices, IProjectServices projectServices, IRoleInProjectServices roleInProjectServices, ITaskServices taskServices, IStatusServices statusServices, string project, DetailAppUser user, string ownerRole)
        {
            _applicationUserServices = applicationUserServices;
            _memberInTaskServices = memberInTaskServices;
            _positionInProjectServices = positionInProjectServices;
            _positionWorkOfMemberServices = positionWorkOfMemberServices;
            _roleApplicationUserServices = roleApplicationUserServices;
            _projectServices = projectServices;
            _roleInProjectServices = roleInProjectServices;
            _taskServices = taskServices;
            _statusServices = statusServices;
            InitializeOwnerRole();
            InitializeStatuses();
        }



        #region Check and Get User
        /// <summary>
        /// Validates the user ID, retrieves the user from the data source, and returns the result.
        /// </summary>
        /// <param name="userId">The user ID or email to check and retrieve.</param>
        /// <returns>A service result containing the retrieved user details or an appropriate failure message.</returns>
        public async Task<ServicesResult<DetailAppUser>> CheckAndGetUser(string userId)
        {
            // Validate the input parameter
            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<DetailAppUser>.Failure("User ID cannot be null or empty.");
            }

            // Attempt to retrieve the user by ID or email
            var userResult = await _applicationUserServices.GetAppUserByIdOrEmail(userId);

            // Check if the service call was successful
            if (!userResult.Status)
            {
                return ServicesResult<DetailAppUser>.Failure("Unable to contact the database.");
            }

            // Check if the user data exists
            if (userResult.Data == null)
            {
                return ServicesResult<DetailAppUser>.Success(_user, "User not found.");
            }

            // Initialize the retrieved user for this logic
            _user = userResult.Data;

            // Return the successful result with the retrieved user data
            return ServicesResult<DetailAppUser>.Success(userResult.Data, string.Empty);
        }
        #endregion

        #region check project is existed by project id
        /// <summary>
        /// Checks whether a project exists in the database based on the given project ID.
        /// </summary>
        /// <param name="projectId">The ID of the project to check.</param>
        /// <returns>A service result indicating whether the project exists.</returns>
        public async Task<ServicesResult<bool>> CheckProjectIsExisted(string projectId)
        {
            // Validate the project ID
            if (string.IsNullOrEmpty(projectId))
                return ServicesResult<bool>.Failure("Project ID is required.");

            // Attempt to retrieve the project from the database
            var projectResult = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (!projectResult.Status)
                return ServicesResult<bool>.Failure("Unable to connect to the database.");

            if (projectResult.Data == null)
                return ServicesResult<bool>.Success(false, "No project found with the given ID.");

            // Project exists
            return ServicesResult<bool>.Success(true, string.Empty);
        }

        #endregion

        #region get members in project
        /// <summary>
        /// Retrieves the list of members in a specific project, including their roles and positions.
        /// </summary>
        /// <returns>A service result containing a collection of members in the project.</returns>
        public async Task<ServicesResult<IEnumerable<IndexMember>>> GetMembersInProject()
        {
            var membersList = new List<IndexMember>();

            // Fetch all role-application user relationships
            var getMembers = await _roleApplicationUserServices.GetAllAsync();
            if (!getMembers.Status)
                return ServicesResult<IEnumerable<IndexMember>>.Failure(getMembers.Message);

            if (getMembers.Data == null)
                return ServicesResult<IEnumerable<IndexMember>>.Success(membersList, "No members found in the database.");

            // Filter members associated with the current project
            var projectMembers = getMembers.Data.Where(x => x.ProjectId == _project).ToList();
            if (!projectMembers.Any())
                return ServicesResult<IEnumerable<IndexMember>>.Success(membersList, "No members found in this project.");

            // Verify if the current user is part of the project
            var userInProject = getMembers.Data.Any(x => x.ApplicationUserId == _user.UserId);
            if (!userInProject)
                return ServicesResult<IEnumerable<IndexMember>>.Failure("The user has not joined this project.");

            // Process each member in the project
            foreach (var member in projectMembers)
            {
                // Retrieve user details
                var userDetails = await _applicationUserServices.GetAppUserByIdOrEmail(member.ApplicationUserId);
                if (!userDetails.Status)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure("Unable to retrieve user details from the database.");

                if (userDetails.Data == null)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure("User details not found.");

                // Fetch the position work assignments
                var positionWorks = await _positionWorkOfMemberServices.GetAllAsync();
                if (!positionWorks.Status)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure("Unable to retrieve position work assignments from the database.");

                if (positionWorks.Data == null)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure("No position work assignments found for members.");

                // Get the position work for the current member
                var userPositionWork = positionWorks.Data.FirstOrDefault(x => x.RoleApplicationUserInProjectId == member.Id);
                if (userPositionWork == null)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure($"User {userDetails.Data.UserName} does not have a position assigned in this project.");

                // Retrieve the position details
                var position = await _positionInProjectServices.GetValueByPrimaryKeyAsync(userPositionWork.PostitionInProjectId);
                if (!position.Status)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure("Unable to retrieve position details from the database.");

                if (position.Data == null)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure($"Position details for user {userDetails.Data.UserName} not found.");

                // Construct the member information
                var memberInfo = new IndexMember
                {
                    UserName = userDetails.Data.UserName,
                    PositionWorkName = position.Data.PositionName,
                    RoleUserInProjectId = member.Id,
                    UserAvata = userDetails.Data.Avata
                };

                membersList.Add(memberInfo);
            }

            return ServicesResult<IEnumerable<IndexMember>>.Success(membersList, string.Empty);
        }
        #endregion

        #region get member by member id
        /// <summary>
        /// Retrieves detailed information about a member in a project by their member ID.
        /// </summary>
        /// <param name="memberId">The ID of the member to retrieve.</param>
        /// <returns>A service result containing the member's details.</returns>
        /// <summary>
        /// Retrieves the details of a member by their member ID within the current project.
        /// </summary>
        /// <param name="memberId">The ID of the member to retrieve.</param>
        /// <returns>A service result containing the detailed member information or an error message.</returns>
        public async Task<ServicesResult<DetailMember>> GetMemberByMemberId(string memberId)
        {
            // Initialize an empty DetailMember object
            var data = new DetailMember();

            // Validate the input member ID
            if (string.IsNullOrEmpty(memberId))
                return ServicesResult<DetailMember>.Failure("Member ID is required.");

            // Retrieve all role-application user relationships
            var getMembers = await _roleApplicationUserServices.GetAllAsync();
            if (!getMembers.Status)
                return ServicesResult<DetailMember>.Failure(getMembers.Message);

            if (getMembers.Data == null)
                return ServicesResult<DetailMember>.Success(data, "No members found in the database.");

            // Check if the current user is part of the project
            var userInProject = getMembers.Data.Any(x => x.ProjectId == _project && x.ApplicationUserId == _user.UserId);
            if (!userInProject)
                return ServicesResult<DetailMember>.Success(data, "The user is not part of this project.");

            // Find the specified member within the project
            var memberInProject = getMembers.Data.FirstOrDefault(x => x.ProjectId == _project && x.ApplicationUserId == memberId);
            if (memberInProject == null)
                return ServicesResult<DetailMember>.Failure("Member not found in this project.");

            // Retrieve member details
            var memberDetails = await _applicationUserServices.GetAppUserByIdOrEmail(memberInProject.ApplicationUserId);
            if (!memberDetails.Status)
                return ServicesResult<DetailMember>.Failure("Failed to retrieve member details from the database.");
            if (memberDetails.Data == null)
                return ServicesResult<DetailMember>.Success(data, "Member not found in the database.");

            // Populate basic member details
            data = new DetailMember
            {
                RoleUserInProjectId = memberId,
                UserName = memberDetails.Data.UserName,
                UserAvata = memberDetails.Data.Avata,
            };

            // Retrieve the role information of the member
            var roleDetails = await _roleInProjectServices.GetValueByPrimaryKeyAsync(memberInProject.RoleInProjectId);
            if (!roleDetails.Status)
                return ServicesResult<DetailMember>.Failure(roleDetails.Message);
            if (roleDetails.Data == null)
                return ServicesResult<DetailMember>.Failure("No roles found in the database.");

            // Assign the role name to the member details
            data.RoleUserNameInProject = roleDetails.Data.RoleName;

            // Retrieve position work assignments
            var positionWorks = await _positionWorkOfMemberServices.GetAllAsync();
            if (!positionWorks.Status)
                return ServicesResult<DetailMember>.Failure("Unable to retrieve position work assignments from the database.");
            if (positionWorks.Data == null)
                return ServicesResult<DetailMember>.Failure("No position work assignments found for members.");

            var userPositionWork = positionWorks.Data.FirstOrDefault(x => x.RoleApplicationUserInProjectId == memberId);
            if (userPositionWork == null)
                return ServicesResult<DetailMember>.Failure($"User {memberDetails.Data.UserName} does not have a position assigned in this project.");

            // Retrieve and populate tasks associated with the member
            var getTasks = await _memberInTaskServices.GetAllAsync();
            if (!getTasks.Status)
                return ServicesResult<DetailMember>.Failure("Failed to retrieve tasks from the database.");
            if (getTasks.Data == null)
                return ServicesResult<DetailMember>.Success(data, "No tasks found in the database.");

            var tasksOfUser = getTasks.Data.Where(x => x.PositionWorkOfMemberId == userPositionWork.Id).ToList();
            if (tasksOfUser.Count == 0)
                return ServicesResult<DetailMember>.Success(data, "This member has no tasks assigned.");

            foreach (var task in tasksOfUser)
            {
                // Retrieve task details
                var taskDetails = await _taskServices.GetValueByPrimaryKeyAsync(task.TaskId);
                if (!taskDetails.Status)
                    return ServicesResult<DetailMember>.Failure(taskDetails.Message);
                if (taskDetails.Data == null)
                    return ServicesResult<DetailMember>.Success(data, "Failed to retrieve task details for the user.");

                // Retrieve the task's status
                var statusInfo = _statuses.FirstOrDefault(x => x.Id == taskDetails.Data.StatusId);
                if (statusInfo == null)
                    return ServicesResult<DetailMember>.Failure("Failed to retrieve the status of the task.");

                // Add the task details to the member's task list
                var indexTask = new IndexTask
                {
                    TaskName = taskDetails.Data.TaskName,
                    Status = statusInfo.Value,
                    TaskId = task.TaskId,
                };
                data.Tasks.Add(indexTask);
            }

            return ServicesResult<DetailMember>.Success(data, string.Empty);
        }


        #endregion

        #region add new member
        /// <summary>
        /// Adds a member to a project after performing necessary validations.
        /// </summary>
        /// <param name="appUserId">The ID of the application user to add.</param>
        /// <param name="addMember">Details about the member being added.</param>
        /// <returns>A service result indicating the success or failure of the operation.</returns>
        public async Task<ServicesResult<bool>> Add(string appUserId, AddMember addMember)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(appUserId) || addMember == null)
                return ServicesResult<bool>.Failure("Invalid input. User ID or AddMember details are missing.");

            // Retrieve all roles from the database
            var rolesResult = await _roleInProjectServices.GetAllAsync();
            if (!rolesResult.Status)
                return ServicesResult<bool>.Failure(rolesResult.Message);

            if (rolesResult.Data == null)
                return ServicesResult<bool>.Success(false, "No roles found in the database.");

            // Find the "Leader" role ID
            var leaderRoleId = rolesResult.Data.FirstOrDefault(x => x.RoleName == "Leader")?.Id;
            if (leaderRoleId == null)
                return ServicesResult<bool>.Success(false, "Leader role not found in the database.");

            // Retrieve all members in the project
            var membersResult = await _roleApplicationUserServices.GetAllAsync();
            if (!membersResult.Status)
                return ServicesResult<bool>.Failure(membersResult.Message);

            if (membersResult.Data == null)
                return ServicesResult<bool>.Success(false, "No members found in the project.");

            // Check if the user is already a member of the project
            var isMemberExists = membersResult.Data.Any(x => x.ApplicationUserId == appUserId);
            if (isMemberExists)
                return ServicesResult<bool>.Success(false, "The member already exists in the project.");

            // Add the new member to the project
            var addMemberResult = await AddMember(appUserId, addMember);
            if (!addMemberResult.Status)
                return ServicesResult<bool>.Failure(addMemberResult.Message);

            return ServicesResult<bool>.Success(true, "Member successfully added to the project.");
        }

        #endregion



        #region private method helper
        /// <summary>
        /// Retrieves the ID of the "Owner" role in projects.
        /// </summary>
        private async Task<ServicesResult<string>> GetOwnerRole()
        {
            var rolesResult = await _roleInProjectServices.GetAllAsync();

            if (!rolesResult.Status)
                return ServicesResult<string>.Failure(rolesResult.Message);

            var ownerRoleId = rolesResult.Data?.FirstOrDefault(x => x.RoleName == "Owner")?.Id;

            return ownerRoleId != null
                ? ServicesResult<string>.Success(ownerRoleId, string.Empty)
                : ServicesResult<string>.Failure("Owner role not found.");
        }

        /// <summary>
        /// Initializes the owner's role by retrieving it from the database and caching the result.
        /// </summary>
        private void InitializeOwnerRole()
        {
            ServicesResult<string> roleResult;

            do
            {
                // Retrieve the owner role until successful
                roleResult = GetOwnerRole().GetAwaiter().GetResult();
            } while (!roleResult.Status || roleResult.Data == null);

            _ownerRole = roleResult.Data;
        }

        /// <summary>
        /// Adds a new member to a project and assigns a position.
        /// </summary>
        /// <param name="appUserId">The ID of the application user to add.</param>
        /// <param name="addMember">Details about the member being added.</param>
        /// <returns>A service result indicating the success or failure of the operation.</returns>
        private async Task<ServicesResult<bool>> AddMember(string appUserId, AddMember addMember)
        {
            // Validate input parameters
            if (addMember == null)
                return ServicesResult<bool>.Failure("AddMember object cannot be null.");

            // Generate a random ID for new entities
            var randomId = new Random().Next(1000000, 9999999);
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            // Check if the user exists in the database
            var userCheck = await _applicationUserServices.GetAppUserByIdOrEmail(appUserId);
            if (!userCheck.Status)
                return ServicesResult<bool>.Failure(userCheck.Message);

            if (userCheck.Data == null)
                return ServicesResult<bool>.Success(false, "User not found in the database.");

            // Create a new RoleApplicationUserInProject entity
            var member = new RoleApplicationUserInProject
            {
                ApplicationUserId = userCheck.Data.UserId,
                Id = $"1003-{randomId}-{timestamp}",
                ProjectId = _project,
                RoleInProjectId = _ownerRole
            };

            // Retrieve all positions in the project
            var positionsResult = await _positionInProjectServices.GetAllAsync();
            if (!positionsResult.Status)
                return ServicesResult<bool>.Failure(positionsResult.Message);

            if (positionsResult.Data == null)
                return ServicesResult<bool>.Success(false, "No positions found in the database.");

            // Check if the specified position exists for the project
            var position = positionsResult.Data.FirstOrDefault(
                x => x.PositionName == addMember.PositionWorkName && x.ProjectId == addMember.ProjectId);

            if (position == null)
                return ServicesResult<bool>.Success(false, "The specified position does not exist.");

            // Create a new PositionWorkOfMember entity
            var positionWork = new PositionWorkOfMember
            {
                Id = $"1005-{randomId}-{timestamp}",
                PostitionInProjectId = position.Id,
                RoleApplicationUserInProjectId = member.Id,
            };

            // Add the new member and position work to the database
            var memberAddResult = await _roleApplicationUserServices.AddAsync(member);
            var positionWorkAddResult = await _positionWorkOfMemberServices.AddAsync(positionWork);

            // Check the results of the database operations
            if (!memberAddResult.Status || !positionWorkAddResult.Status)
            {
                var errorMessage = memberAddResult.Message ?? positionWorkAddResult.Message;
                return ServicesResult<bool>.Failure($"Failed to add member or position work: {errorMessage}");
            }

            return ServicesResult<bool>.Success(true, "Member successfully added.");
        }
        /// <summary>
        /// Retrieves all statuses from the database.
        /// </summary>
        /// <returns>A service result containing a list of statuses or an error message.</returns>
        private async Task<ServicesResult<IEnumerable<Status>>> GetStatusesAsync()
        {
            // Initialize an empty list to store statuses
            var result = new List<Status>();

            // Retrieve all statuses from the database
            var statusResult = await _statusServices.GetAllAsync();

            // Check if data is null or the service call failed
            if (statusResult.Data == null)
                return ServicesResult<IEnumerable<Status>>.Success(result, "No statuses found in the database.");
            if (!statusResult.Status)
                return ServicesResult<IEnumerable<Status>>.Failure(statusResult.Message);

            // Populate the result with retrieved data
            result = statusResult.Data.ToList();
            return ServicesResult<IEnumerable<Status>>.Success(result, string.Empty);
        }

        /// <summary>
        /// Initializes the status list by fetching all statuses from the database.
        /// </summary>
        private void InitializeStatuses()
        {
            // Continuously attempt to fetch statuses until successful
            ServicesResult<IEnumerable<Status>> statusResult;
            do
            {
                // Retrieve statuses synchronously by awaiting the task
                statusResult = GetStatusesAsync().GetAwaiter().GetResult();
            }
            while (statusResult.Data == null || !statusResult.Status);

            // Assign the fetched statuses to the class-level variable
            _statuses = statusResult.Data.ToList();
        }


        #endregion
    }
}
