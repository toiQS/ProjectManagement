using PM.Domain;
using PM.DomainServices.ILogic;
using PM.Persistence.IServices;
using Shared;
using Shared.member;
using Shared.task;

namespace PM.DomainServices.Logic
{
    public class MemberLogic : IMemberLogic
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

        public MemberLogic(
            IApplicationUserServices applicationUserServices,
            IMemberInTaskServices memberInTaskServices,
            IPositionInProjectServices positionInProjectServices,
            IPositionWorkOfMemberServices positionWorkOfMemberServices,
            IRoleApplicationUserInProjectServices roleApplicationUserServices,
            IProjectServices projectServices,
            IRoleInProjectServices roleInProjectServices,
            ITaskServices taskServices,
            IStatusServices statusServices)
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
        }
        #region GetMembersInProject Method
        /// <summary>
        /// Retrieves a list of members in a specific project.
        /// </summary>
        /// <param name="userId">The ID of the user making the request.</param>
        /// <param name="projectId">The ID of the project for which members are being retrieved.</param>
        /// <returns>
        /// A <see cref="ServicesResult{T}"/> containing a list of <see cref="IndexMember"/> objects if successful, or a failure message otherwise.
        /// </returns>
        public async Task<ServicesResult<IEnumerable<IndexMember>>> GetMembersInProject(string userId, string projectId)
        {
            #region Input Validation
            // Validate userId and projectId are not null or empty
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<IEnumerable<IndexMember>>.Failure("Invalid userId or projectId.");
            #endregion

            #region Check User and Project Existence
            // Ensure the user exists
            if ((await _applicationUserServices.GetUser(userId)) == null)
                return ServicesResult<IEnumerable<IndexMember>>.Failure("User not found.");

            // Ensure the project exists
            if ((await _projectServices.GetValueByPrimaryKeyAsync(projectId)) == null)
                return ServicesResult<IEnumerable<IndexMember>>.Failure("Project not found.");

            // Ensure the user has a role in the project
            var userRolesInProject = (await _roleApplicationUserServices.GetAllAsync())
                .Any(x => x.ApplicationUserId == userId && x.ProjectId == projectId);

            if (!userRolesInProject)
                return ServicesResult<IEnumerable<IndexMember>>.Failure("User does not have a role in the project.");
            #endregion

            #region Retrieve Members in Project
            // Get all members associated with the project
            var membersInProject = (await _roleApplicationUserServices.GetAllAsync())
                .Where(x => x.ProjectId == projectId);

            if (!membersInProject.Any())
                return ServicesResult<IEnumerable<IndexMember>>.Failure("No members found in the project.");
            #endregion

            #region Build Member Data
            var data = new List<IndexMember>();

            foreach (var member in membersInProject)
            {
                // Fetch user details
                var user = await _applicationUserServices.GetUser(member.ApplicationUserId);
                if (user == null)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure("A member's user data is missing.");

                // Fetch position work details
                var positionWork = (await _positionWorkOfMemberServices.GetAllAsync())
                    .FirstOrDefault(x => x.RoleApplicationUserInProjectId == member.Id);

                if (positionWork == null)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure("A member's position work data is missing.");

                // Fetch position details
                var position = await _positionInProjectServices.GetValueByPrimaryKeyAsync(positionWork.PostitionInProjectId);
                if (position == null)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure("A member's position data is missing.");

                // Add member data to the list
                data.Add(new IndexMember
                {
                    RoleUserInProjectId = member.Id,
                    UserName = user.UserName,
                    PositionWorkName = position.PositionName,
                    UserAvata = user.PathImage,
                });
            }
            #endregion

            return ServicesResult<IEnumerable<IndexMember>>.Success(data);
        }
        #endregion

        #region GetMemberByMemberId Method
        /// <summary>
        /// Retrieves detailed information about a specific project member by their member ID.
        /// </summary>
        /// <param name="userId">The ID of the user making the request.</param>
        /// <param name="memberId">The ID of the project member to retrieve.</param>
        /// <returns>
        /// A <see cref="ServicesResult{T}"/> containing a <see cref="DetailMember"/> object if successful, or a failure message otherwise.
        /// </returns>
        public async Task<ServicesResult<DetailMember>> GetMemberByMemberId(string userId, string memberId)
        {
            #region Input Validation
            // Validate userId and memberId are not null or empty
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(memberId))
                return ServicesResult<DetailMember>.Failure("Invalid userId or memberId.");
            #endregion

            #region Check User and Role Existence
            // Ensure the user exists and has roles assigned
            if ((await _applicationUserServices.GetUser(userId)) == null ||
                !(await _roleApplicationUserServices.GetAllAsync()).Any(x => x.ApplicationUserId == userId))
                return ServicesResult<DetailMember>.Failure("User not found or has no assigned roles.");
            #endregion

            #region Retrieve Member Details
            // Fetch role details for the member
            var roleUser = await _roleApplicationUserServices.GetValueByPrimaryKeyAsync(memberId);
            if (roleUser == null)
                return ServicesResult<DetailMember>.Failure("Member not found.");

            // Fetch user details
            var user = await _applicationUserServices.GetUser(roleUser.ApplicationUserId);
            if (user == null)
                return ServicesResult<DetailMember>.Failure("Member's user data is missing.");

            // Fetch position work details
            var positionWork = (await _positionWorkOfMemberServices.GetAllAsync())
                .FirstOrDefault(x => x.RoleApplicationUserInProjectId == roleUser.Id);

            if (positionWork == null)
                return ServicesResult<DetailMember>.Failure("Position work data is missing.");

            // Fetch position details
            var position = await _positionInProjectServices.GetValueByPrimaryKeyAsync(positionWork.PostitionInProjectId);
            if (position == null)
                return ServicesResult<DetailMember>.Failure("Position data is missing.");

            // Fetch role details
            var role = await _roleInProjectServices.GetValueByPrimaryKeyAsync(roleUser.RoleInProjectId);
            if (role == null)
                return ServicesResult<DetailMember>.Failure("Role data is missing.");

            // Create the DetailMember object
            var data = new DetailMember
            {
                RoleUserInProjectId = roleUser.Id,
                RoleUserNameInProject = role.RoleName,
                UserAvata = user.PathImage,
                UserName = user.UserName,
            };
            #endregion

            #region Retrieve Tasks Assigned to the Member
            // Fetch tasks assigned to the member
            var memberTasks = (await _memberInTaskServices.GetAllAsync())
                .Where(x => x.PositionWorkOfMemberId == positionWork.Id);

            if (memberTasks.Any())
            {
                var taskList = new List<IndexTask>();

                foreach (var memberTask in memberTasks)
                {
                    var task = await _taskServices.GetValueByPrimaryKeyAsync(memberTask.Id);
                    if (task == null)
                        return ServicesResult<DetailMember>.Failure("Task data is missing.");

                    var status = (await _statusServices.GetAllAsync()).FirstOrDefault(x => x.Id == task.StatusId)?.Value;
                    if (status == null)
                        return ServicesResult<DetailMember>.Failure("Task status is missing.");

                    taskList.Add(new IndexTask
                    {
                        TaskId = task.Id,
                        TaskName = task.TaskName,
                        Status = status,
                    });
                }

                // Assign tasks to the DetailMember object
                data.Tasks = taskList;
            }
            #endregion

            return ServicesResult<DetailMember>.Success(data);
        }
        #endregion

        #region Add Method
        /// <summary>
        /// Adds a new member to a project with a specified role and position.
        /// </summary>
        /// <param name="userId">The ID of the user performing the addition.</param>
        /// <param name="addMember">The information about the member being added.</param>
        /// <param name="projectId">The ID of the project the member will be added to.</param>
        /// <returns>A <see cref="ServicesResult{T}"/> indicating success or failure of the operation.</returns>
        public async Task<ServicesResult<bool>> Add(string userId, AddMember addMember, string projectId)
        {
            #region Input Validation
            // Validate inputs for null or empty values
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId) || addMember == null)
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            #endregion

            #region Validate User and Permissions
            // Retrieve Role IDs for "Owner" and "Leader"
            var getRoleOwnerId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            var getRoleLeaderId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Leader")?.Id;

            // Ensure the user exists and has the necessary permissions in the project (Owner or Leader role)
            if ((await _applicationUserServices.GetUser(userId)) == null ||
                !(await _roleApplicationUserServices.GetAllAsync())
                    .Any(x => x.ProjectId == projectId
                            && x.ApplicationUserId == userId
                            && (x.RoleInProjectId == getRoleOwnerId || x.RoleInProjectId == getRoleLeaderId)))
                return ServicesResult<bool>.Failure("User is not authorized to add members.");
            #endregion

            #region Handle Member Addition
            ApplicationUser user = new ApplicationUser();
            var random = new Random().Next(1000000, 9999999); // Generate a random ID for new entities

            // Check if the member is being added by email
            if (addMember.ValueUser.Contains("@"))
            {
                user = await _applicationUserServices.GetUserByEmail(addMember.ValueUser);
                if (user == null)
                    return ServicesResult<bool>.Failure("User not found by email.");
            }

            // Ensure the member is not already assigned to the project
            if ((await _roleApplicationUserServices.GetAllAsync()).Any(x => x.ProjectId == projectId && x.ApplicationUserId == user.Id))
                return ServicesResult<bool>.Failure("User is already a member of this project.");

            // Create the role for the new user in the project
            var roleUser = new RoleApplicationUserInProject()
            {
                Id = $"1003-{random}-{DateTime.Now}",
                ProjectId = projectId,
                RoleInProjectId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == addMember.RoleUserNameInProject)?.Id,
                ApplicationUserId = user.Id
            };

            if (!(await _roleApplicationUserServices.AddAsync(roleUser)))
                return ServicesResult<bool>.Failure("Failed to assign role to the user.");

            // Assign a position to the user in the project
            var positionId = (await _positionInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PositionName == addMember.PositionWorkName)?.Id;
            if (positionId == null)
                return ServicesResult<bool>.Failure("Position not found in the project.");

            var positionWork = new PositionWorkOfMember()
            {
                Id = $"1005-{random}-{DateTime.Now}",
                PostitionInProjectId = positionId,
                RoleApplicationUserInProjectId = roleUser.Id
            };

            if (!(await _positionWorkOfMemberServices.AddAsync(positionWork)))
                return ServicesResult<bool>.Failure("Failed to assign position to the user.");
            #endregion

            return ServicesResult<bool>.Success(true);
        }
        #endregion


        #region UpdateInfo Method
        /// <summary>
        /// Updates the role and position information of a member in a project.
        /// </summary>
        /// <param name="userId">The ID of the user performing the update.</param>
        /// <param name="memberId">The ID of the member whose information is being updated.</param>
        /// <param name="updateMember">The updated member information (role and position).</param>
        /// <returns>
        /// A <see cref="ServicesResult{T}"/> indicating the success or failure of the update operation.
        /// </returns>
        public async Task<ServicesResult<bool>> UpdateInfo(string userId, string memberId, UpdateMember updateMember)
        {
            #region Input Validation
            // Validate that the inputs are not null or empty
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(memberId) || updateMember == null)
                return ServicesResult<bool>.Failure("Invalid input parameters.");
            #endregion

            #region Validate User Permissions
            // Get the role IDs for "Owner" and "Leader"
            var getRoleOwnerId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            var getRoleLeaderId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Leader")?.Id;

            // Ensure the user exists and has sufficient permissions (Owner or Leader role)
            if ((await _applicationUserServices.GetUser(userId)) == null ||
                !(await _roleApplicationUserServices.GetAllAsync())
                    .Any(x => x.ApplicationUserId == userId
                            && (x.RoleInProjectId == getRoleOwnerId || x.RoleInProjectId == getRoleLeaderId)))
                return ServicesResult<bool>.Failure("User does not have permission to update member information.");
            #endregion

            #region Retrieve Role and Position Information
            // Retrieve the updated role ID and position ID from the input
            var getRoleId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == updateMember.RoleUserInProjectName)?.Id;
            var getPositionId = (await _positionInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PositionName == updateMember.PositionNameInProject)?.Id;

            // Validate that the role and position exist
            if (getRoleId == null || getPositionId == null)
                return ServicesResult<bool>.Failure("Invalid role or position name provided.");
            #endregion

            #region Update Role and Position for the Member
            // Retrieve the role assignment for the specified member
            var getRoleUser = await _roleApplicationUserServices.GetValueByPrimaryKeyAsync(memberId);
            if (getRoleUser == null)
                return ServicesResult<bool>.Failure("Member not found.");

            // Update the member's role
            getRoleUser.RoleInProjectId = getRoleId;
            if (!(await _roleApplicationUserServices.UpdateAsync(memberId, getRoleUser)))
                return ServicesResult<bool>.Failure("Failed to update member role.");

            // Retrieve the position work information for the specified member
            var getPositionWork = (await _positionWorkOfMemberServices.GetAllAsync())
                .FirstOrDefault(x => x.RoleApplicationUserInProjectId == getRoleUser.Id);
            if (getPositionWork == null)
                return ServicesResult<bool>.Failure("Position work not found for the member.");

            // Update the member's position
            getPositionWork.PostitionInProjectId = getPositionId;
            if (!await _positionWorkOfMemberServices.UpdateAsync(getPositionWork.Id, getPositionWork))
                return ServicesResult<bool>.Failure("Failed to update member position.");
            #endregion

            // Return success if all updates were successful
            return ServicesResult<bool>.Success(true);
        }
        #endregion


        #region Delete Method
        /// <summary>
        /// Deletes a member from a project, including their role and position assignments.
        /// </summary>
        /// <param name="userId">The ID of the user performing the deletion.</param>
        /// <param name="memberId">The ID of the member to be deleted.</param>
        /// <returns>
        /// A <see cref="ServicesResult{T}"/> indicating the success or failure of the deletion operation.
        /// </returns>
        public async Task<ServicesResult<bool>> Delete(string userId, string memberId)
        {
            #region Input Validation
            // Validate that the userId and memberId are not null or empty
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(memberId))
                return ServicesResult<bool>.Failure("User ID or Member ID cannot be empty.");
            #endregion

            #region Validate User Permissions
            // Get the role IDs for "Owner" and "Leader"
            var getRoleOwnerId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            var getRoleLeaderId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Leader")?.Id;

            // Ensure the user exists and has the necessary permissions (Owner or Leader role)
            if ((await _applicationUserServices.GetUser(userId)) == null ||
                !(await _roleApplicationUserServices.GetAllAsync())
                    .Any(x => x.ApplicationUserId == userId
                            && (x.RoleInProjectId == getRoleOwnerId || x.RoleInProjectId == getRoleLeaderId)))
                return ServicesResult<bool>.Failure("User does not have permission to delete members.");
            #endregion

            #region Retrieve Role and Position Information
            // Retrieve the role information for the specified member
            var roleUser = await _roleApplicationUserServices.GetValueByPrimaryKeyAsync(memberId);
            if (roleUser == null)
                return ServicesResult<bool>.Failure("Member not found.");

            // Retrieve the position work information for the specified member
            var getPositionWork = (await _positionWorkOfMemberServices.GetAllAsync())
                .FirstOrDefault(x => x.RoleApplicationUserInProjectId == memberId);
            #endregion

            #region Handle Deletion of Position and Tasks
            // If the member has no position work, directly attempt to delete the role assignment
            if (getPositionWork == null)
            {
                if (await _roleApplicationUserServices.DeleteAsync(memberId))
                    return ServicesResult<bool>.Success(true);
                return ServicesResult<bool>.Failure("Failed to delete the member's role.");
            }

            // Retrieve tasks assigned to this position work (if any)
            var taskMember = (await _memberInTaskServices.GetAllAsync())
                .Where(x => x.PositionWorkOfMemberId == getPositionWork.Id);
            #endregion

            #region Handle Task Deletion and Cleanup
            // If there are tasks assigned, delete them first
            if (taskMember != null)
            {
                foreach (var task in taskMember)
                {
                    if (!await _memberInTaskServices.DeleteAsync(task.Id))
                        return ServicesResult<bool>.Failure("Failed to delete member's tasks.");
                }
            }

            // Delete the position work assignment
            if (!await _positionWorkOfMemberServices.DeleteAsync(getPositionWork.Id))
                return ServicesResult<bool>.Failure("Failed to delete position work.");

            #endregion

            #region Final Deletion of Role
            // Finally, delete the role assignment of the member
            if (await _roleApplicationUserServices.DeleteAsync(memberId))
                return ServicesResult<bool>.Success(true);

            return ServicesResult<bool>.Failure("Failed to delete the member.");
            #endregion
        }
        #endregion

    }
}
