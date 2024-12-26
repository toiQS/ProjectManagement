using PM.Domain;
using PM.DomainServices.ILogic;
using PM.Persistence.IServices;
using Shared;
using Shared.project;

namespace PM.DomainServices.Logic
{
    internal class ProjectLogic : IProjectLogic
    {
        private readonly IApplicationUserServices _applicationUserServices;
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserServices;
        private readonly IProjectServices _projectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        private readonly IStatusServices _statusServices;
        private readonly IMemberLogic _memberLogic;
        private readonly IPlanLogic _planLogic;
        private readonly IPositionLogic _positionLogic;
        public ProjectLogic(IApplicationUserServices applicationUserServices, IRoleApplicationUserInProjectServices roleApplicationUserServices, IProjectServices projectServices, IRoleInProjectServices roleInProjectServices, IStatusServices statusServices, IMemberLogic memberLogic, IPlanLogic planLogic, IPositionLogic position)
        {
            _applicationUserServices = applicationUserServices;
            _roleApplicationUserServices = roleApplicationUserServices;
            _projectServices = projectServices;
            _roleInProjectServices = roleInProjectServices;
            _statusServices = statusServices;
            _memberLogic = memberLogic;
            _planLogic = planLogic;
            _positionLogic = position;
        }

        #region GetProjectListUserHasJoined
        /// <summary>
        /// Retrieves the list of projects a user has joined.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A service result containing a list of projects the user has joined.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectListUserHasJoined(string userId)
        {
            // Validate user ID
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<IEnumerable<IndexProject>>.Failure("User ID cannot be null or empty.");

            // Check if the user exists
            var user = await _applicationUserServices.GetUserDetailByUserId(userId);
            if (user == null)
                return ServicesResult<IEnumerable<IndexProject>>.Failure("User not found.");

            // Get the list of projects the user is part of
            var userProjects = (await _roleApplicationUserServices.GetAllAsync()).Where(x => x.ApplicationUserId == userId);
            if (!userProjects.Any())
                return ServicesResult<IEnumerable<IndexProject>>.Failure("No projects found for the user.");

            // Retrieve the "Owner" role ID
            var ownerRole = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner");
            if (ownerRole == null)
                return ServicesResult<IEnumerable<IndexProject>>.Failure("Owner role not found.");

            var ownerRoleId = ownerRole.Id;

            // Initialize the result list
            var projectList = new List<IndexProject>();

            // Iterate through each project the user is part of
            foreach (var projectUser in userProjects)
            {
                // Skip deleted projects
                var project = await _projectServices.GetValueByPrimaryKeyAsync(projectUser.ProjectId);
                if (project == null || project.IsDeleted)
                    continue;

                // Get the owner of the project
                var projectOwner = (await _roleApplicationUserServices.GetAllAsync())
                    .FirstOrDefault(x => x.RoleInProjectId == ownerRoleId && x.ProjectId == projectUser.ProjectId);
                if (projectOwner == null)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure("Project owner not found.");

                var ownerUser = await _applicationUserServices.GetUserDetailByUserId(projectOwner.ApplicationUserId);
                if (ownerUser == null)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure("Owner user not found.");

                // Construct the project data
                var projectItem = new IndexProject
                {
                    ProjectId = project.Id,
                    ProjectName = project.ProjectName,
                    OwnerName = ownerUser.UserName,
                    OwnerAvata = ownerUser.PathImage
                };

                projectList.Add(projectItem);
            }

            // Return the list of projects
            return ServicesResult<IEnumerable<IndexProject>>.Success(projectList);
        }
        #endregion

        #region GetProjectListUserHasOwner
        /// <summary>
        /// Retrieves a list of projects where the specified user is the owner.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A service result containing a list of projects owned by the user.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectListUserHasOwner(string userId)
        {
            // Validate user ID
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<IEnumerable<IndexProject>>.Failure("User ID cannot be null or empty.");

            // Check if the user exists
            var user = await _applicationUserServices.GetUserDetailByUserId(userId);
            if (user == null)
                return ServicesResult<IEnumerable<IndexProject>>.Failure("User not found.");

            // Retrieve the "Owner" role ID
            var ownerRole = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner");
            if (ownerRole == null)
                return ServicesResult<IEnumerable<IndexProject>>.Failure("Owner role not found.");

            var ownerRoleId = ownerRole.Id;

            // Get all projects where the user is the owner
            var ownedProjects = (await _roleApplicationUserServices.GetAllAsync())
                .Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == ownerRoleId);

            if (!ownedProjects.Any())
                return ServicesResult<IEnumerable<IndexProject>>.Failure("No owned projects found for the user.");

            // Prepare the result list
            var projectList = new List<IndexProject>();

            // Iterate through the owned projects
            foreach (var projectRole in ownedProjects)
            {
                // Fetch the project details
                var project = await _projectServices.GetValueByPrimaryKeyAsync(projectRole.ProjectId);
                if (project == null || project.IsDeleted)
                    continue; // Skip null or deleted projects

                // Add the project to the result list
                var projectData = new IndexProject
                {
                    ProjectId = project.Id,
                    ProjectName = project.ProjectName,
                    OwnerName = user.UserName,
                    OwnerAvata = user.PathImage
                };

                projectList.Add(projectData);
            }

            // Return the list of owned projects
            return ServicesResult<IEnumerable<IndexProject>>.Success(projectList);
        }
        #endregion

        #region GetProjectDetailProjectHasJoined
        /// <summary>
        /// Retrieves detailed information about a project that the user has joined.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>A service result containing the project details.</returns>
        public async Task<ServicesResult<DetailProject>> GetProjectDetailProjectHasJoined(string userId, string projectId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<DetailProject>.Failure("User ID or Project ID cannot be null or empty.");

            // Check if the user is part of the project
            var projectUserRoles = (await _roleApplicationUserServices.GetAllAsync())
                .Where(x => x.ApplicationUserId == userId && x.ProjectId == projectId);

            if (!projectUserRoles.Any())
                return ServicesResult<DetailProject>.Failure("User is not part of the project.");

            // Fetch the project details
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (project == null)
                return ServicesResult<DetailProject>.Failure("Project not found.");

            // Check if the user has access to the project
            var ownerRoleId = (await _roleInProjectServices.GetAllAsync())
                .FirstOrDefault(x => x.RoleName == "Owner")?.Id;

            if (ownerRoleId == null)
                return ServicesResult<DetailProject>.Failure("Owner role not found.");

            if (!project.IsAccessed && !projectUserRoles.Any(x => x.RoleInProjectId == ownerRoleId))
                return ServicesResult<DetailProject>.Failure("User does not have access to this project.");

            // Retrieve project owner details
            var projectOwnerRole = (await _roleApplicationUserServices.GetAllAsync())
                .FirstOrDefault(x => x.ProjectId == projectId && x.RoleInProjectId == ownerRoleId);

            if (projectOwnerRole == null)
                return ServicesResult<DetailProject>.Failure("Project owner not found.");

            var ownerUser = await _applicationUserServices.GetUserDetailByUserId(projectOwnerRole.ApplicationUserId);
            if (ownerUser == null)
                return ServicesResult<DetailProject>.Failure("Owner user details not found.");

            // Prepare the project detail data
            var projectDetails = new DetailProject
            {
                ProjectId = projectId,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                CreateAt = project.CreateAt,
                StartAt = project.StartAt,
                EndAt = project.EndAt,
                IsAccessed = project.IsAccessed,
                IsDeleted = project.IsDeleted,
                IsDone = project.IsDone,
                Status = (await _statusServices.GetAllAsync())
                    .FirstOrDefault(x => x.Id == project.StatusId)?.Value ?? "Unknown",
                QuantityMember = (await _roleApplicationUserServices.GetAllAsync())
                    .Count(x => x.ProjectId == projectId),
                OwnerName = ownerUser.UserName,
                OwnerAvata = ownerUser.PathImage
            };

            // Fetch and attach project members
            var membersResult = await _memberLogic.GetMembersInProject(userId, projectId);
            if (membersResult.Data == null)
                return ServicesResult<DetailProject>.Failure("Failed to fetch project members.");

            projectDetails.Members = membersResult.Data.Select(member => new Shared.member.IndexMember
            {
                PositionWorkName = member.PositionWorkName,
                UserName = member.UserName,
                RoleUserInProjectId = userId,
                UserAvata = ownerUser.PathImage
            }).ToList();

            // Return the project details
            return ServicesResult<DetailProject>.Success(projectDetails);
        }
        #endregion

        #region AddProject
        /// <summary>
        /// Adds a new project if the user is authorized (not already an owner of a project with the same name).
        /// </summary>
        /// <param name="userId">The ID of the user trying to add the project.</param>
        /// <param name="addProject">The project details to be added.</param>
        /// <returns>A service result indicating success or failure.</returns>
        public async Task<ServicesResult<bool>> Add(string userId, AddProject addProject)
        {
            if (string.IsNullOrEmpty(userId) || addProject == null)
                return ServicesResult<bool>.Failure("Invalid parameters.");

            var ownerRoleId = (await _roleInProjectServices.GetAllAsync())
                                .FirstOrDefault(x => x.RoleName == "Owner")?.Id;

            if (ownerRoleId == null)
                return ServicesResult<bool>.Failure("Owner role not found.");

            // Check if the user is already an owner of a project with the same name
            var userProjects = (await _roleApplicationUserServices.GetAllAsync())
                                .Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == ownerRoleId);

            if (userProjects.Any())
            {
                foreach (var project in userProjects)
                {
                    var existingProject = await _projectServices.GetValueByPrimaryKeyAsync(project.ProjectId);
                    if (existingProject != null && existingProject.ProjectName == addProject.ProjectName)
                    {
                        return ServicesResult<bool>.Failure("Project with the same name already exists.");
                    }
                }
            }

            // Proceed with adding the project
            return await AddMethod(userId, addProject);
        }
        #endregion

        #region AddMethod
        /// <summary>
        /// Adds the project and assigns the user as the owner.
        /// </summary>
        /// <param name="userId">The ID of the user to be assigned as the owner.</param>
        /// <param name="addProject">The project details to be added.</param>
        /// <returns>A service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> AddMethod(string userId, AddProject addProject)
        {
            var random = new Random().Next(1000000, 9000000);

            var project = new Project
            {
                Id = $"1001-{random}-{DateTime.Now}",
                ProjectName = addProject.ProjectName,
                CreateAt = DateTime.Now,
                EndAt = addProject.EndAt,
                IsAccessed = true,
                IsDeleted = false,
                IsDone = false,
                ProjectDescription = addProject.ProjectDescription,
                StartAt = addProject.StartAt,
                StatusId = DateTime.Now == addProject.StartAt ? 3 : (DateTime.Now < addProject.StartAt ? 2 : 1) // Conditional status assignment
            };

            if (!await _projectServices.AddAsync(project))
                return ServicesResult<bool>.Failure("Failed to create the project.");

            var ownerRoleId = (await _roleInProjectServices.GetAllAsync())
                                .FirstOrDefault(x => x.RoleName == "Owner")?.Id;

            if (ownerRoleId == null)
                return ServicesResult<bool>.Failure("Owner role not found.");

            var roleProject = new RoleApplicationUserInProject
            {
                Id = $"1002-{random}-{DateTime.Now}",
                ProjectId = project.Id,
                ApplicationUserId = userId,
                RoleInProjectId = ownerRoleId
            };

            if (await _roleApplicationUserServices.AddAsync(roleProject))
            {
                return ServicesResult<bool>.Success(true);
            }

            return ServicesResult<bool>.Failure("Failed to assign the user as project owner.");
        }
        #endregion

        #region UpdateProjectInfo
        /// <summary>
        /// Updates the project information, but only if the user is the owner of the project.
        /// </summary>
        /// <param name="userId">The ID of the user trying to update the project.</param>
        /// <param name="projectId">The ID of the project to update.</param>
        /// <param name="updateProject">The updated project details.</param>
        /// <returns>A service result indicating success or failure.</returns>
        public async Task<ServicesResult<bool>> UpdateInfo(string userId, string projectId, UpdateProject updateProject)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId) || updateProject == null)
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            // Get the 'Owner' role ID
            var getRoleOwner = (await _roleInProjectServices.GetAllAsync())
                                .FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            if (getRoleOwner == null)
                return ServicesResult<bool>.Failure("Owner role not found.");

            // Check if the user is the owner of the specified project
            var projectUser = (await _roleApplicationUserServices.GetAllAsync())
                                .Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == getRoleOwner && x.ProjectId == projectId);
            if (!projectUser.Any())
                return ServicesResult<bool>.Failure("User is not the owner of the project.");

            // Get the project to be updated
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (project == null)
                return ServicesResult<bool>.Failure("Project not found.");

            // Check if the project name is the same as the one being updated (no changes)
            if (project.ProjectName == updateProject.ProjectName)
                return ServicesResult<bool>.Failure("No changes to the project name.");

            // Update project details
            project.ProjectName = updateProject.ProjectName;
            project.ProjectDescription = updateProject.ProjectDescription;

            // Save the updated project information
            if (await _projectServices.UpdateAsync(project))
                return ServicesResult<bool>.Success(true);

            // Return failure if update fails
            return ServicesResult<bool>.Failure("Failed to update the project.");
        }
        #endregion
        #region Helper Method
        // Helper method to check if the user is the owner and get the project.
        private async Task<(Project project, bool isOwner)> GetProjectAndCheckOwnerAsync(string userId, string projectId)
        {
            var getRoleOwner = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            if (getRoleOwner == null) return (null, false);

            var projectUser = (await _roleApplicationUserServices.GetAllAsync())
                              .Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == getRoleOwner && x.ProjectId == projectId);

            if (!projectUser.Any()) return (null, false);

            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (project == null) return (null, false);

            return (project, true);
        }
        #endregion

        #region UpdateIsDelete
        /// <summary>
        /// Toggles the deletion status of a project. 
        /// This operation can only be performed by the project owner.
        /// </summary>
        /// <param name="userId">The user ID of the person attempting to toggle the deletion status.</param>
        /// <param name="projectId">The ID of the project to be updated.</param>
        /// <returns>A result indicating whether the update was successful or not.</returns>
        public async Task<ServicesResult<bool>> UpdateIsDelete(string userId, string projectId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<bool>.Failure("User or project ID cannot be null or empty.");

            // Get project and check if user is the owner
            var (project, isOwner) = await GetProjectAndCheckOwnerAsync(userId, projectId);
            if (!isOwner || project == null)
                return ServicesResult<bool>.Failure("User is not the owner or project not found.");

            // Toggle the IsDeleted flag
            project.IsDeleted = !project.IsDeleted;

            // Update the project status
            if (await _projectServices.UpdateAsync(project))
                return ServicesResult<bool>.Success(true);

            return ServicesResult<bool>.Failure("Failed to update project deletion status.");
        }
        #endregion

        #region UpdateIsAccessed
        /// <summary>
        /// Toggles the access status of a project. 
        /// This operation can only be performed by the project owner.
        /// </summary>
        /// <param name="userId">The user ID of the person attempting to toggle the access status.</param>
        /// <param name="projectId">The ID of the project to be updated.</param>
        /// <returns>A result indicating whether the update was successful or not.</returns>
        public async Task<ServicesResult<bool>> UpdateIsAccessed(string userId, string projectId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<bool>.Failure("User or project ID cannot be null or empty.");

            // Get project and check if user is the owner
            var (project, isOwner) = await GetProjectAndCheckOwnerAsync(userId, projectId);
            if (!isOwner || project == null)
                return ServicesResult<bool>.Failure("User is not the owner or project not found.");

            // Toggle the IsAccessed flag
            project.IsAccessed = !project.IsAccessed;

            // Update the project status
            if (await _projectServices.UpdateAsync(project))
                return ServicesResult<bool>.Success(true);

            return ServicesResult<bool>.Failure("Failed to update project access status.");
        }
        #endregion

        #region UpdateIsDone
        /// <summary>
        /// Toggles the completion status of a project. 
        /// This operation can only be performed by the project owner.
        /// </summary>
        /// <param name="userId">The user ID of the person attempting to toggle the completion status.</param>
        /// <param name="projectId">The ID of the project to be updated.</param>
        /// <returns>A result indicating whether the update was successful or not.</returns>
        public async Task<ServicesResult<bool>> UpdateIsDone(string userId, string projectId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<bool>.Failure("User or project ID cannot be null or empty.");

            // Get project and check if user is the owner
            var (project, isOwner) = await GetProjectAndCheckOwnerAsync(userId, projectId);
            if (!isOwner || project == null)
                return ServicesResult<bool>.Failure("User is not the owner or project not found.");

            // Toggle the IsDone flag
            project.IsDone = !project.IsDone;

            // Update the project status
            if (!await _projectServices.UpdateAsync(project))
                return ServicesResult<bool>.Failure("Failed to update project completion status.");

            // After toggling IsDone, update project status
            return await UpdateStatus(userId, projectId);
        }
        #endregion

        #region UpdateStatus
        /// <summary>
        /// Updates the status of a project based on its completion and end date.
        /// This operation can only be performed by the project owner.
        /// </summary>
        /// <param name="userId">The user ID of the person attempting to update the project status.</param>
        /// <param name="projectId">The ID of the project to be updated.</param>
        /// <returns>A result indicating whether the update was successful or not.</returns>
        public async Task<ServicesResult<bool>> UpdateStatus(string userId, string projectId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<bool>.Failure("User or project ID cannot be null or empty.");

            // Get project and check if user is the owner
            var (project, isOwner) = await GetProjectAndCheckOwnerAsync(userId, projectId);
            if (!isOwner || project == null)
                return ServicesResult<bool>.Failure("User is not the owner or project not found.");

            // Correct assignment mistakes in conditions (using == instead of =)
            if (!project.IsDone && project.EndAt < DateTime.Now)
                project.StatusId = 6; // Ended but not done
            else if (!project.IsDone && project.EndAt == DateTime.Now)
                project.StatusId = 5; // Ended today but not done
            else if (!project.IsDone && project.EndAt > DateTime.Now)
                project.StatusId = 3; // Active but not done
            else if (project.IsDone && project.EndAt < DateTime.Now)
                project.StatusId = 7; // Completed and ended
            else if (project.IsDone && project.EndAt == DateTime.Now)
                project.StatusId = 5; // Completed, ending today
            else if (project.IsDone && project.EndAt > DateTime.Now)
                project.StatusId = 4; // Completed, still ongoing

            // Update the project status
            if (await _projectServices.UpdateAsync(project))
                return ServicesResult<bool>.Success(true);

            return ServicesResult<bool>.Failure("Failed to update project status.");
        }
        #endregion


        #region Delete Method
        /// <summary>
        /// Deletes a project along with its associated plans, members, and positions.
        /// This method ensures that only the project owner can delete the project.
        /// </summary>
        /// <param name="userId">The user ID of the person attempting to delete the project.</param>
        /// <param name="projectId">The ID of the project to be deleted.</param>
        /// <returns>A result indicating whether the deletion was successful or not.</returns>
        public async Task<ServicesResult<bool>> Delete(string userId, string projectId)
        {
            #region Validate Input Parameters
            // Ensure that userId and projectId are not null or empty
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<bool>.Failure("User ID or Project ID cannot be null or empty.");
            #endregion

            #region Check if User is the Project Owner
            // Get the 'Owner' role ID
            var getRoleOwner = (await _roleInProjectServices.GetAllAsync())
                .FirstOrDefault(x => x.RoleName == "Owner")?.Id;

            // If 'Owner' role is not found, return failure
            if (getRoleOwner == null)
                return ServicesResult<bool>.Failure("Owner role not found.");

            // Check if the user is the owner of the project
            var projectUser = (await _roleApplicationUserServices.GetAllAsync())
                .Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == getRoleOwner);

            // If the user is not the owner, return failure
            if (!projectUser.Any())
                return ServicesResult<bool>.Failure("User is not the owner of the project.");
            #endregion

            #region Retrieve Project Information
            // Retrieve project details
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            // If project is not found, return failure
            if (project == null)
                return ServicesResult<bool>.Failure("Project not found.");
            #endregion

            #region Delete Associated Plans
            // Attempt to delete associated plans
            var planDeleteResult = await _planLogic.Delete(userId, projectId);
            if (!planDeleteResult.Status)
                return ServicesResult<bool>.Failure("Failed to delete associated plans.");
            #endregion

            #region Delete Associated Members
            // Attempt to delete associated members
            var memberDeleteResult = await _memberLogic.Delete(userId, projectId);
            if (!memberDeleteResult.Status)
                return ServicesResult<bool>.Failure("Failed to delete associated members.");
            #endregion

            #region Delete Associated Positions
            // Retrieve associated positions for the project
            var positions = (await _positionLogic.Get(userId, projectId))?.Data;

            // If positions are found, attempt to delete each position
            if (positions != null && positions.Any())
            {
                foreach (var position in positions)
                {
                    var positionDeleteResult = await _positionLogic.Delete(userId, position.PositionId, projectId);
                    if (!positionDeleteResult.Status)
                        return ServicesResult<bool>.Failure($"Failed to delete position with ID: {position.PositionId}");
                }
            }
            else
            {
                return ServicesResult<bool>.Failure("No positions found to delete.");
            }
            #endregion

            #region Delete the Project
            // Finally, delete the project itself
            if (await _projectServices.DeleteAsync(projectId))
                return ServicesResult<bool>.Success(true);

            // If project deletion fails, return failure
            return ServicesResult<bool>.Failure("Failed to delete project.");
            #endregion
        }
        #endregion

    }
}
