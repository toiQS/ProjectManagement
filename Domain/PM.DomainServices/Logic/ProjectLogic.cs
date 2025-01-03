using Azure.Core;
using Microsoft.AspNetCore.Components.Forms;
using PM.Domain;
using PM.DomainServices.ILogic;
using PM.Persistence.IServices;
using Shared;
using Shared.member;
using Shared.project;
using System.Numerics;

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
        /// <returns>A service result containing the list of projects.</returns>
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
            var getProjects = await _roleApplicationUserServices.GetAllAsync();
            if (getProjects.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Failure(getProjects.Message);
            var userProjects = getProjects.Data.Where(x => x.ApplicationUserId == userId);
            if (!userProjects.Any())
                return ServicesResult<IEnumerable<IndexProject>>.Success(new List<IndexProject> { });

            // Retrieve the "Owner" role ID
            var getRoles = await _roleInProjectServices.GetAllAsync();
            if (getRoles.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Failure(getRoles.Message);
            var ownerRole = getRoles.Data.FirstOrDefault(x => x.RoleName == "Owner");
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
                if (!project.Status || project.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Failure(project.Message);
                if (project.Status && project.Data.IsDeleted)
                    continue;

                // Get the owner of the project
                var getRole = await _roleApplicationUserServices.GetAllAsync();
                if (getRole.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Failure(getRole.Message);
                var projectOwner = getRole.Data
                    .FirstOrDefault(x => x.RoleInProjectId == ownerRoleId && x.ProjectId == projectUser.ProjectId);

                if (projectOwner == null)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure("Project owner not found.");

                var ownerUser = await _applicationUserServices.GetUserDetailByUserId(projectOwner.ApplicationUserId);
                if (ownerUser == null)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure("Owner user not found.");

                // Construct the project data
                var projectItem = new IndexProject
                {
                    ProjectId = project.Data.Id,
                    ProjectName = project.Data.ProjectName,
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
        /// Retrieves the list of projects owned by a specific user.
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
            var getRoles = (await _roleInProjectServices.GetAllAsync());
            if (getRoles.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Failure(getRoles.Message);
            var ownerRole = getRoles.Data.FirstOrDefault(x => x.RoleName == "Owner");
            if (ownerRole == null)
                return ServicesResult<IEnumerable<IndexProject>>.Failure("Owner role not found.");

            var ownerRoleId = ownerRole.Id;

            // Get all projects where the user is the owner
            var getRoleProjects = (await _roleApplicationUserServices.GetAllAsync());
            if (getRoleProjects.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Failure(getRoleProjects.Message);
            var ownedProjects = getRoleProjects.Data
                .Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == ownerRoleId);

            if (!ownedProjects.Any())
                return ServicesResult<IEnumerable<IndexProject>>.Success(new List<IndexProject> { });

            // Prepare the result list
            var projectList = new List<IndexProject>();

            // Iterate through the owned projects
            foreach (var projectRole in ownedProjects)
            {
                // Fetch the project details
                var project = await _projectServices.GetValueByPrimaryKeyAsync(projectRole.ProjectId);
                if (project.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Failure(project.Message);
                if (!project.Status || project.Data.IsDeleted)
                    continue; // Skip null or deleted projects

                // Add the project to the result list
                var projectData = new IndexProject
                {
                    ProjectId = project.Data.Id,
                    ProjectName = project.Data.ProjectName,
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
        /// Retrieves detailed information about a project that a specific user has joined.
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
            var getRoleProject = (await _roleApplicationUserServices.GetAllAsync());
            if (!getRoleProject.Status) return ServicesResult<DetailProject>.Failure(getRoleProject.Message);
            var projectUserRoles = getRoleProject.Data
                .Where(x => x.ApplicationUserId == userId && x.ProjectId == projectId);

            if (!projectUserRoles.Any())
                return ServicesResult<DetailProject>.Failure("User is not part of the project.");

            // Fetch the project details
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (project.Data == null)
                return ServicesResult<DetailProject>.Failure(project.Message);

            // Check if the user has access to the project
            var getRole = (await _roleInProjectServices.GetAllAsync());
            if (getRole.Data == null) return ServicesResult<DetailProject>.Failure(getRole.Message);
            var ownerRoleId = getRole.Data
                .FirstOrDefault(x => x.RoleName == "Owner")?.Id;

            if (ownerRoleId == null)
                return ServicesResult<DetailProject>.Failure("Owner role not found.");

            if (!project.Data.IsAccessed && !projectUserRoles.Any(x => x.RoleInProjectId == ownerRoleId))
                return ServicesResult<DetailProject>.Failure("User does not have access to this project.");

            // Retrieve project owner details
            var getProjects = (await _roleApplicationUserServices.GetAllAsync());
            if (getProjects.Data == null) return ServicesResult<DetailProject>.Failure(getProjects.Message);
            var projectOwnerRole = getProjects.Data
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
                ProjectName = project.Data.ProjectName,
                ProjectDescription = project.Data.ProjectDescription,
                CreateAt = project.Data.CreateAt,
                StartAt = project.Data.StartAt,
                EndAt = project.Data.EndAt,
                IsAccessed = project.Data.IsAccessed,
                IsDeleted = project.Data.IsDeleted,
                IsDone = project.Data.IsDone,
                Status = (await _statusServices.GetAllAsync()).Data
                    .FirstOrDefault(x => x.Id == project.Data.StatusId)?.Value ?? "Unknown",
                QuantityMember = (await _roleApplicationUserServices.GetAllAsync()).Data
                    .Count(x => x.ProjectId == projectId),
                OwnerName = ownerUser.UserName,
                OwnerAvata = ownerUser.PathImage,
                Members = new List<IndexMember>(),
                Plans = new List<Shared.plan.IndexPlan>()
            };

            // Fetch and attach project members
            var membersResult = await _memberLogic.GetMembersInProject(userId, projectId);
            if (membersResult.Data == null)
                return ServicesResult<DetailProject>.Success(projectDetails);

            projectDetails.Members = membersResult.Data.Select(member => new Shared.member.IndexMember
            {
                PositionWorkName = member.PositionWorkName,
                UserName = member.UserName,
                RoleUserInProjectId = userId,
                UserAvata = ownerUser.PathImage
            }).ToList();

            var tasksResult = await _planLogic.GetPlansInProjectId(userId, projectId);
            if (tasksResult.Data == null)
                return ServicesResult<DetailProject>.Success(projectDetails);
            projectDetails.Plans = tasksResult.Data.Select(x => new Shared.plan.IndexPlan()
            {
                PlanName = x.PlanName,
                PlanId = x.PlanId,
                Status = x.Status,
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
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || addProject == null)
                return ServicesResult<bool>.Failure("Invalid parameters.");

            // Retrieve the owner role ID
            var getRole = await _roleInProjectServices.GetAllAsync();
            if (getRole.Data == null)
                return ServicesResult<bool>.Failure(getRole.Message);

            var ownerRoleId = getRole.Data.FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            if (ownerRoleId == null)
                return ServicesResult<bool>.Failure("Owner role not found.");

            // Check if the user is already an owner of a project with the same name
            var getRoleProject = await _roleApplicationUserServices.GetAllAsync();
            if (getRoleProject.Data == null)
                return ServicesResult<bool>.Failure(getRoleProject.Message);

            var userProjects = getRoleProject.Data
                .Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == ownerRoleId);

            foreach (var project in userProjects)
            {
                var existingProject = await _projectServices.GetValueByPrimaryKeyAsync(project.ProjectId);
                if (!existingProject.Status || existingProject.Data == null)
                    return ServicesResult<bool>.Failure(existingProject.Message);

                if (existingProject.Data.ProjectName == addProject.ProjectName)
                {
                    return ServicesResult<bool>.Failure("Project with the same name already exists.");
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
            // Generate a unique ID for the project
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

            // Add the project to the database
            var resultAdd = await _projectServices.AddAsync(project);
            if (!resultAdd.Status)
                return ServicesResult<bool>.Failure($"Failed to create the project. {resultAdd.Message}");

            // Retrieve the owner role ID
            var getRoles = await _roleInProjectServices.GetAllAsync();
            if (getRoles.Data == null)
                return ServicesResult<bool>.Failure(getRoles.Message);

            var ownerRoleId = getRoles.Data.FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            if (ownerRoleId == null)
                return ServicesResult<bool>.Failure("Owner role not found.");

            // Assign the user as the owner of the project
            var roleProject = new RoleApplicationUserInProject
            {
                Id = $"1002-{random}-{DateTime.Now}",
                ProjectId = project.Id,
                ApplicationUserId = userId,
                RoleInProjectId = ownerRoleId
            };

            var addRoleResult = await _roleApplicationUserServices.AddAsync(roleProject);
            if (addRoleResult.Status)
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
            var getRoles = (await _roleInProjectServices.GetAllAsync());
            if(getRoles.Data == null) return ServicesResult<bool>.Failure(getRoles.Message);
            var getRoleOwner = getRoles.Data
                                .FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            if (getRoleOwner == null)
                return ServicesResult<bool>.Failure("Owner role not found.");

            // Check if the user is the owner of the specified project
            var getProjects = (await _roleApplicationUserServices.GetAllAsync());
            if (getProjects.Data == null) return ServicesResult<bool>.Failure(getProjects.Message);
            var projectUser = getProjects.Data
                                .Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == getRoleOwner && x.ProjectId == projectId);
            if (!projectUser.Any())
                return ServicesResult<bool>.Failure("User is not the owner of the project.");

            // Get the project to be updated
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (project.Data == null)
                return ServicesResult<bool>.Failure("Project not found.");

            // Check if the project name is the same as the one being updated (no changes)

            if (project.Data.ProjectName == updateProject.ProjectName)
                return ServicesResult<bool>.Failure("No changes to the project name.");

            // Update project details
            project.Data.ProjectName = updateProject.ProjectName;
            project.Data.ProjectDescription = updateProject.ProjectDescription;

            // Save the updated project information
            if ((await _projectServices.UpdateAsync(project.Data)).Status)
                return ServicesResult<bool>.Success(true);

            // Return failure if update fails
            return ServicesResult<bool>.Failure("Failed to update the project.");
        }
        #endregion
        // Helper Method to Check Ownership and Retrieve Project
        #region Helper Methods
        /// <summary>
        /// Helper method to check if the user is the owner and retrieve the project.
        /// </summary>
        /// <param name="userId">The user ID of the person attempting the operation.</param>
        /// <param name="projectId">The ID of the project to be checked.</param>
        /// <returns>A tuple containing the project and a boolean indicating ownership status.</returns>
        private async Task<(Project project, bool isOwner)> GetProjectAndCheckOwnerAsync(string userId, string projectId)
        {
            var rolesResult = await _roleInProjectServices.GetAllAsync();
            if (rolesResult.Data == null) return (null, false);

            var ownerRoleId = rolesResult.Data.FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            if (ownerRoleId == null) return (null, false);

            var userRolesResult = await _roleApplicationUserServices.GetAllAsync();
            if (userRolesResult.Data == null) return (null, false);

            var isOwner = userRolesResult.Data.Any(x => x.ApplicationUserId == userId && x.RoleInProjectId == ownerRoleId && x.ProjectId == projectId);
            if (!isOwner) return (null, false);

            var projectResult = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            return projectResult.Data != null ? (projectResult.Data, true) : (null, false);
        }
        #endregion

        // Toggle Deletion Status
        #region Update IsDeleted
        /// <summary>
        /// Toggles the deletion status of a project. This operation can only be performed by the project owner.
        /// </summary>
        /// <param name="userId">The user ID of the person attempting to toggle the deletion status.</param>
        /// <param name="projectId">The ID of the project to be updated.</param>
        /// <returns>A result indicating whether the update was successful.</returns>
        public async Task<ServicesResult<bool>> UpdateIsDeletedAsync(string userId, string projectId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<bool>.Failure("User or project ID cannot be null or empty.");

            var (project, isOwner) = await GetProjectAndCheckOwnerAsync(userId, projectId);
            if (!isOwner || project == null)
                return ServicesResult<bool>.Failure("User is not the owner or project not found.");

            project.IsDeleted = !project.IsDeleted;
            var updateResult = await _projectServices.UpdateAsync(project);
            return updateResult.Status
                ? ServicesResult<bool>.Success(true)
                : ServicesResult<bool>.Failure($"Failed to update project deletion status. {updateResult.Message}");
        }
        #endregion

        // Toggle Access Status
        #region Update IsAccessed
        /// <summary>
        /// Toggles the access status of a project. This operation can only be performed by the project owner.
        /// </summary>
        /// <param name="userId">The user ID of the person attempting to toggle the access status.</param>
        /// <param name="projectId">The ID of the project to be updated.</param>
        /// <returns>A result indicating whether the update was successful.</returns>
        public async Task<ServicesResult<bool>> UpdateIsAccessedAsync(string userId, string projectId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<bool>.Failure("User or project ID cannot be null or empty.");

            var (project, isOwner) = await GetProjectAndCheckOwnerAsync(userId, projectId);
            if (!isOwner || project == null)
                return ServicesResult<bool>.Failure("User is not the owner or project not found.");

            project.IsAccessed = !project.IsAccessed;
            var updateResult = await _projectServices.UpdateAsync(project);
            return updateResult.Status
                ? ServicesResult<bool>.Success(true)
                : ServicesResult<bool>.Failure($"Failed to update project access status. {updateResult.Message}");
        }
        #endregion

        // Toggle Completion Status
        #region Update IsDone
        /// <summary>
        /// Toggles the completion status of a project. This operation can only be performed by the project owner.
        /// </summary>
        /// <param name="userId">The user ID of the person attempting to toggle the completion status.</param>
        /// <param name="projectId">The ID of the project to be updated.</param>
        /// <returns>A result indicating whether the update was successful.</returns>
        public async Task<ServicesResult<bool>> UpdateIsDoneAsync(string userId, string projectId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<bool>.Failure("User or project ID cannot be null or empty.");

            var (project, isOwner) = await GetProjectAndCheckOwnerAsync(userId, projectId);
            if (!isOwner || project == null)
                return ServicesResult<bool>.Failure("User is not the owner or project not found.");

            project.IsDone = !project.IsDone;
            var updateResult = await _projectServices.UpdateAsync(project);
            if (!updateResult.Status)
                return ServicesResult<bool>.Failure("Failed to update project completion status.");

            return await UpdateProjectStatusAsync(userId, projectId);
        }
        #endregion

        // Update Project Status
        #region Update Project Status
        /// <summary>
        /// Updates the status of a project based on its completion and end date.
        /// </summary>
        /// <param name="userId">The user ID of the person attempting to update the project status.</param>
        /// <param name="projectId">The ID of the project to be updated.</param>
        /// <returns>A result indicating whether the update was successful.</returns>
        public async Task<ServicesResult<bool>> UpdateProjectStatusAsync(string userId, string projectId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<bool>.Failure("User or project ID cannot be null or empty.");

            var (project, isOwner) = await GetProjectAndCheckOwnerAsync(userId, projectId);
            if (!isOwner || project == null)
                return ServicesResult<bool>.Failure("User is not the owner or project not found.");

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

            var updateResult = await _projectServices.UpdateAsync(project);
            return updateResult.Status
                ? ServicesResult<bool>.Success(true)
                : ServicesResult<bool>.Failure("Failed to update project status.");
        }
        #endregion

        // Delete Project
        #region Delete Project
        /// <summary>
        /// Deletes a project along with its associated plans, members, and positions. This operation can only be performed by the project owner.
        /// </summary>
        /// <param name="userId">The user ID of the person attempting to delete the project.</param>
        /// <param name="projectId">The ID of the project to be deleted.</param>
        /// <returns>A result indicating whether the deletion was successful.</returns>
        public async Task<ServicesResult<bool>> DeleteProjectAsync(string userId, string projectId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<bool>.Failure("User ID or Project ID cannot be null or empty.");

            var rolesResult = await _roleInProjectServices.GetAllAsync();
            if (rolesResult.Data == null) return ServicesResult<bool>.Failure(rolesResult.Message);

            var ownerRoleId = rolesResult.Data.FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            if (ownerRoleId == null)
                return ServicesResult<bool>.Failure("Owner role not found.");

            var userRolesResult = await _roleApplicationUserServices.GetAllAsync();
            if (userRolesResult.Data == null) return ServicesResult<bool>.Failure(userRolesResult.Message);

            var isOwner = userRolesResult.Data.Any(x => x.ApplicationUserId == userId && x.RoleInProjectId == ownerRoleId);
            if (!isOwner)
                return ServicesResult<bool>.Failure("User is not the owner of the project.");

            var projectResult = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (projectResult.Data == null)
                return ServicesResult<bool>.Failure("Project not found.");

            var planDeleteResult = await _planLogic.Delete(userId, projectId);
            if (!planDeleteResult.Status)
                return ServicesResult<bool>.Failure("Failed to delete associated plans.");

            var memberDeleteResult = await _memberLogic.Delete(userId, projectId);
            if (!memberDeleteResult.Status)
                return ServicesResult<bool>.Failure("Failed to delete associated members.");

            var positions = (await _positionLogic.Get(userId, projectId))?.Data;
            if (positions != null && positions.Any())
            {
                foreach (var position in positions)
                {
                    var positionDeleteResult = await _positionLogic.Delete(userId, position.PositionId, projectId);
                    if (!positionDeleteResult.Status)
                        return ServicesResult<bool>.Failure($"Failed to delete position with ID: {position.PositionId}");
                }
            }

            var projectDeleteResult = await _projectServices.DeleteAsync(projectId);
            return projectDeleteResult.Status
                ? ServicesResult<bool>.Success(true)
                : ServicesResult<bool>.Failure("Failed to delete project.");
        }
        #endregion


    }
}
