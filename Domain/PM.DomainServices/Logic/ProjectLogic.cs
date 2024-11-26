using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.IdentityModel.Tokens;
using PM.Domain;
using PM.DomainServices.ILogic;
using PM.DomainServices.OutDataDTOs;
using PM.DomainServices.Shared;
using PM.Persistence.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace PM.DomainServices.Logic
{
    public class ProjectLogic
    {
        private readonly IApplicationUserServices _applicationUserServices;
        private readonly IProjectServices _projectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        private readonly IPositionInProjectServices _positionInProjectServices;
        private readonly IPositionWorkOfMemberServices _positionWorkOfMemberServices;
        private readonly IPlanInProjectServices _planInProjectServices;
        private readonly ITaskInPlanServices _taskInPlanServices;
        private readonly ITaskServices _taskServices;
        private readonly IPlanServices _planServices;
        private readonly IMemberInTaskServices _memberInTaskServices;
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserInProjectServices;

        // Constructor for dependency injection
        public ProjectLogic(
            IProjectServices projectServices,
            IRoleInProjectServices roleInProjectServices,
            IPositionInProjectServices positionInProjectServices,
            IPlanInProjectServices planInProjectServices,
            ITaskInPlanServices taskInPlanServices,
            ITaskServices taskServices,
            IPlanServices planServices,
            IMemberInTaskServices memberInTaskServices,
            IRoleApplicationUserInProjectServices roleApplicationUserInProjectServices,
            IApplicationUserServices applicationUserServices,
            IPositionWorkOfMemberServices positionWorkOfMemberServices)
        {
            _applicationUserServices = applicationUserServices;
            _projectServices = projectServices;
            _roleInProjectServices = roleInProjectServices;
            _positionInProjectServices = positionInProjectServices;
            _planInProjectServices = planInProjectServices;
            _taskInPlanServices = taskInPlanServices;
            _taskServices = taskServices;
            _planServices = planServices;
            _memberInTaskServices = memberInTaskServices;
            _roleApplicationUserInProjectServices = roleApplicationUserInProjectServices;
            _positionWorkOfMemberServices = positionWorkOfMemberServices;
        }

        #region Get Projects User Has Joined
        /// <summary>
        /// Retrieves the list of projects a user has joined.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <returns>A list of ProjectDTO objects if successful, otherwise failure.</returns>
        public async Task<ServicesResult<IEnumerable<ProjectDTO>>> GetProjectsUserHasJoined(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<IEnumerable<ProjectDTO>>.Failure("User ID is invalid.");

            var roleApplicationUserInProjects = await _roleApplicationUserInProjectServices.GetAllAsync();
            if (roleApplicationUserInProjects == null)
                return ServicesResult<IEnumerable<ProjectDTO>>.Failure("No roles found.");

            var userProjects = roleApplicationUserInProjects
                .Where(x => x.ApplicationUserId == userId)
                .ToList();
            if (!userProjects.Any())
                return ServicesResult<IEnumerable<ProjectDTO>>.Failure("User has not joined any projects.");

            var resultProjects = new List<ProjectDTO>();

            // Iterate over the projects and fetch details
            foreach (var item in userProjects)
            {
                var project = await _projectServices.GetValueByPrimaryKeyAsync(item.ProjectId);
                if (project == null) continue;

                var role = await _roleInProjectServices.GetValueByPrimaryKeyAsync(item.RoleInProjectId);
                if (role == null || role.RoleName != "Owner") continue;

                var user = await _applicationUserServices.GetUser(userId);
                if (user == null) continue;

                resultProjects.Add(new ProjectDTO
                {
                    Id = project.Id,
                    Name = project.ProjectName,
                    OwnerName = user.UserName,
                    OwnerImage = user.PathImage,
                });
            }

            return ServicesResult<IEnumerable<ProjectDTO>>.Success(resultProjects);
        }
        #endregion

        #region Add Project
        /// <summary>
        /// Adds a new project for a user.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="projectName">Project name.</param>
        /// <param name="projectDescription">Project description.</param>
        /// <param name="projectVersion">Project version.</param>
        /// <param name="projectStatus">Project status.</param>
        /// <returns>Success or failure of the operation.</returns>
        public async Task<ServicesResult<bool>> AddProject(
            string userId,
            string projectName,
            string projectDescription,
            string projectVersion,
            string projectStatus)
        {
            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(projectName) ||
                string.IsNullOrEmpty(projectDescription) ||
                string.IsNullOrEmpty(projectVersion) ||
                string.IsNullOrEmpty(projectStatus))
                return ServicesResult<bool>.Failure("Invalid input data.");

            var roles = await _roleInProjectServices.GetAllAsync();
            if (roles == null) return ServicesResult<bool>.Failure("Roles not found.");

            var ownerRole = roles.FirstOrDefault(x => x.RoleName == "Owner");
            if (ownerRole == null) return ServicesResult<bool>.Failure("Owner role not found.");

            // Check if the user already has an "Owner" role in a project with the same name
            var roleApplications = await _roleApplicationUserInProjectServices.GetAllAsync();
            if (roleApplications == null) return ServicesResult<bool>.Failure("Role applications not found.");

            var existingProjects = roleApplications
                .Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == ownerRole.Id)
                .ToList();

            foreach (var project in existingProjects)
            {
                var existingProject = await _projectServices.GetValueByPrimaryKeyAsync(project.ProjectId);
                if (existingProject?.ProjectName == projectName)
                    return ServicesResult<bool>.Failure("Project with the same name already exists.");
            }


            return await Add(userId, projectName, projectDescription, projectVersion, projectStatus);
        }
        #endregion

        #region Add Helper Method
        /// <summary>
        /// Helper method to add a new project.
        /// </summary>
        private async Task<ServicesResult<bool>> Add(
            string userId,
            string projectName,
            string projectDescription,
            string projectVersion,
            string projectStatus)
        {
            var randomSuffix = new Random().Next(100000, 999999);

            // Create the project entity
            var project = new Project
            {
                Id = $"1001-{randomSuffix}-{DateTime.Now}",
                ProjectName = projectName,
                CreateAt = DateTime.Now,
                IsAccessed = true,
                IsDeleted = false,
                ProjectDescription = projectDescription,
                Projectstatus = projectStatus,
                ProjectVersion = projectVersion,
            };

            if (!await _projectServices.AddAsync(project))
                return ServicesResult<bool>.Failure("Failed to add project.");

            // Assign the "Owner" role to the user
            var roles = await _roleInProjectServices.GetAllAsync();
            var ownerRole = roles.FirstOrDefault(x => x.RoleName == "Owner");
            if (ownerRole == null) return ServicesResult<bool>.Failure("Owner role not found.");

            var roleUserInProject = new RoleApplicationUserInProject
            {
                RoleInProjectId = ownerRole.Id,
                Id = $"1003-{randomSuffix}-{DateTime.Now}",
                ProjectId = project.Id,
                ApplicationUserId = userId,
            };

            if (!await _roleApplicationUserInProjectServices.AddAsync(roleUserInProject))
                return ServicesResult<bool>.Failure("Failed to assign user role.");

            return ServicesResult<bool>.Success(true);
        }
        #endregion
        #region Update IsDeleted Status of a Project
        /// <summary>
        /// Toggles the "IsDeleted" status of a project if the user has the "Owner" role for the project.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>A <see cref="ServicesResult{bool}"/> indicating success or failure.</returns>
        public async Task<ServicesResult<bool>> UpdateIsDelete(string userId, string projectId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<bool>.Failure("Invalid user or project ID.");

            // Retrieve the "Owner" role
            var roles = await _roleInProjectServices.GetAllAsync();
            var ownerRole = roles.FirstOrDefault(x => x.RoleName == "Owner");
            if (ownerRole == null)
                return ServicesResult<bool>.Failure("Owner role not found.");

            // Verify if the user has the "Owner" role in the specified project
            var roleUserInProjects = await _roleApplicationUserInProjectServices.GetAllAsync();
            var userProject = roleUserInProjects
                .FirstOrDefault(x => x.ProjectId == projectId && x.ApplicationUserId == userId && x.RoleInProjectId == ownerRole.Id);
            if (userProject == null)
                return ServicesResult<bool>.Failure("User is not an owner of the project.");

            // Retrieve the project details
            var project = await _projectServices.GetValueByPrimaryKeyAsync(userProject.ProjectId);
            if (project == null)
                return ServicesResult<bool>.Failure("Project not found.");

            // Toggle the "IsDeleted" status
            project.IsDeleted = !project.IsDeleted;

            // Update the project and return the result
            if (!await _projectServices.UpdateAsync(projectId, project))
                return ServicesResult<bool>.Failure("Failed to update the project.");

            return ServicesResult<bool>.Success(true);
        }
        #endregion

        #region Update IsAccessed Status of a Project
        /// <summary>
        /// Toggles the "IsAccessed" status of a project if the user has the "Owner" role for the project.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>A <see cref="ServicesResult{bool}"/> indicating success or failure.</returns>
        public async Task<ServicesResult<bool>> UpdateIsAccess(string userId, string projectId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<bool>.Failure("Invalid user or project ID.");

            // Retrieve the "Owner" role
            var roles = await _roleInProjectServices.GetAllAsync();
            var ownerRole = roles.FirstOrDefault(x => x.RoleName == "Owner");
            if (ownerRole == null)
                return ServicesResult<bool>.Failure("Owner role not found.");

            // Verify if the user has the "Owner" role in the specified project
            var roleUserInProjects = await _roleApplicationUserInProjectServices.GetAllAsync();
            var userProject = roleUserInProjects
                .FirstOrDefault(x => x.ProjectId == projectId && x.ApplicationUserId == userId && x.RoleInProjectId == ownerRole.Id);
            if (userProject == null)
                return ServicesResult<bool>.Failure("User is not an owner of the project.");

            // Retrieve the project details
            var project = await _projectServices.GetValueByPrimaryKeyAsync(userProject.ProjectId);
            if (project == null)
                return ServicesResult<bool>.Failure("Project not found.");

            // Toggle the "IsAccessed" status
            project.IsAccessed = !project.IsAccessed;

            // Update the project and return the result
            if (!await _projectServices.UpdateAsync(projectId, project))
                return ServicesResult<bool>.Failure("Failed to update the project.");

            return ServicesResult<bool>.Success(true);
        }
        #endregion

        #region Project Management
        /// <summary>
        /// Deletes a project and all its associated components, ensuring only the owner can perform this action.
        /// </summary>
        /// <param name="userId">ID of the user requesting deletion.</param>
        /// <param name="projectId">ID of the project to be deleted.</param>
        /// <returns>Service result indicating success or failure.</returns>
        public async Task<ServicesResult<bool>> DeleteProject(string userId, string projectId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            // Verify the "Owner" role exists
            var roles = await _roleInProjectServices.GetAllAsync();
            var ownerRole = roles.FirstOrDefault(x => x.RoleName == "Owner");
            if (ownerRole == null)
                return ServicesResult<bool>.Failure("Owner role not found.");

            // Verify the user is the owner of the project
            var roleUserInProjects = await _roleApplicationUserInProjectServices.GetAllAsync();
            var isUserOwner = roleUserInProjects.Any(x =>
                x.ProjectId == projectId &&
                x.ApplicationUserId == userId &&
                x.RoleInProjectId == ownerRole.Id);

            if (!isUserOwner)
                return ServicesResult<bool>.Failure("User is not authorized to delete this project.");

            // Fetch associated components
            var membersInProject = roleUserInProjects.Where(x => x.ProjectId == projectId).ToList();
            var positionsInProject = (await _positionInProjectServices.GetAllAsync())
                .Where(x => x.ProjectId == projectId).ToList();
            var plansInProject = (await _planInProjectServices.GetAllAsync())
                .Where(x => x.ProjectId == projectId).ToList();

            // Begin deletion of all project components
            await DeleteProjectComponents(membersInProject, positionsInProject, plansInProject);

            // Delete the project itself
            return await DeleteProjectMethod(projectId);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Deletes all components associated with a project.
        /// </summary>
        private async Task DeleteProjectComponents(
            List<RoleApplicationUserInProject> membersInProject,
            List<PositionInProject> positionsInProject,
            List<PlanInProject> plansInProject)
        {
            // Delete members and their tasks
            foreach (var member in membersInProject)
            {
                foreach (var position in positionsInProject)
                {
                    var positionWorks = (await _positionWorkOfMemberServices.GetAllAsync())
                        .Where(x => x.PostitionInProjectId == position.Id && x.RoleApplicationUserInProjectId == member.Id);

                    foreach (var work in positionWorks)
                    {
                        var tasks = (await _memberInTaskServices.GetAllAsync())
                            .Where(x => x.PositionWorkOfMemberId == work.Id);

                        foreach (var task in tasks)
                        {
                            await DeleteMemberInTaskMethod(task.Id);
                        }

                        await DeletePositionWorkOfMemberMethod(work.Id);
                    }

                    await DeletePositionInProjectMethod(position.Id);
                }

                await DeleteRoleUserInProjectMethod(member.Id);
            }

            // Delete plans and associated tasks
            foreach (var plan in plansInProject)
            {
                var tasksInPlan = (await _taskInPlanServices.GetAllAsync())
                    .Where(x => x.PlanId == plan.Id);

                foreach (var task in tasksInPlan)
                {
                    await DeleteTaskInPlanMethod(task.Id);
                    await DeleteTaskMethod(task.TaskId);
                }

                await DeletePlanInProjectMethod(plan.Id);
                await DeletePlanMethod(plan.ProjectId);
            }
        }
        #endregion

        #region Individual Deletion Methods
        /// <summary>
        /// Deletes the specified project.
        /// </summary>
        private async Task<ServicesResult<bool>> DeleteProjectMethod(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return ServicesResult<bool>.Failure("Invalid project ID.");

            return await _projectServices.DeleteAsync(projectId)
                ? ServicesResult<bool>.Success(true)
                : ServicesResult<bool>.Failure("Failed to delete project.");
        }

        /// <summary>
        /// Deletes a role-user association in a project.
        /// </summary>
        private async Task<ServicesResult<bool>> DeleteRoleUserInProjectMethod(string roleUserId)
        {
            if (string.IsNullOrEmpty(roleUserId))
                return ServicesResult<bool>.Failure("Invalid role-user ID.");

            return await _roleApplicationUserInProjectServices.DeleteAsync(roleUserId)
                ? ServicesResult<bool>.Success(true)
                : ServicesResult<bool>.Failure("Failed to delete role-user association.");
        }

        /// <summary>
        /// Deletes a position in the project.
        /// </summary>
        private async Task<ServicesResult<bool>> DeletePositionInProjectMethod(string positionId)
        {
            if (string.IsNullOrEmpty(positionId))
                return ServicesResult<bool>.Failure("Invalid position ID.");

            return await _positionInProjectServices.DeleteAsync(positionId)
                ? ServicesResult<bool>.Success(true)
                : ServicesResult<bool>.Failure("Failed to delete position.");
        }

        /// <summary>
        /// Deletes a work position assigned to a member.
        /// </summary>
        private async Task<ServicesResult<bool>> DeletePositionWorkOfMemberMethod(string positionWorkOfMemberId)
        {
            if (string.IsNullOrEmpty(positionWorkOfMemberId))
                return ServicesResult<bool>.Failure("Invalid position work ID.");

            return await _positionWorkOfMemberServices.DeleteAsync(positionWorkOfMemberId)
                ? ServicesResult<bool>.Success(true)
                : ServicesResult<bool>.Failure("Failed to delete position work.");
        }
        private async Task<ServicesResult<bool>> DeletePlanMethod(string planId)
        {
            if (string.IsNullOrEmpty(planId))
                return ServicesResult<bool>.Failure("Invalid plan ID.");

            return await _planServices.DeleteAsync(planId)
                ? ServicesResult<bool>.Success(true)
                : ServicesResult<bool>.Failure("Failed to delete plan.");
        }

        /// <summary>
        /// Deletes a task assigned to a member.
        /// </summary>
        private async Task<ServicesResult<bool>> DeleteMemberInTaskMethod(string memberInTaskId)
        {
            if (string.IsNullOrEmpty(memberInTaskId))
                return ServicesResult<bool>.Failure("Invalid task ID.");

            return await _memberInTaskServices.DeleteAsync(memberInTaskId)
                ? ServicesResult<bool>.Success(true)
                : ServicesResult<bool>.Failure("Failed to delete member's task.");
        }

        /// <summary>
        /// Deletes a plan in the project.
        /// </summary>
        private async Task<ServicesResult<bool>> DeletePlanInProjectMethod(string planInProjectId)
        {
            if (string.IsNullOrEmpty(planInProjectId))
                return ServicesResult<bool>.Failure("Invalid plan ID.");

            return await _planInProjectServices.DeleteAsync(planInProjectId)
                ? ServicesResult<bool>.Success(true)
                : ServicesResult<bool>.Failure("Failed to delete plan.");
        }

        /// <summary>
        /// Deletes a task in a plan.
        /// </summary>
        private async Task<ServicesResult<bool>> DeleteTaskInPlanMethod(string taskInPlanId)
        {
            if (string.IsNullOrEmpty(taskInPlanId))
                return ServicesResult<bool>.Failure("Invalid task ID.");

            return await _taskInPlanServices.DeleteAsync(taskInPlanId)
                ? ServicesResult<bool>.Success(true)
                : ServicesResult<bool>.Failure("Failed to delete task in plan.");
        }

        /// <summary>
        /// Deletes a task.
        /// </summary>
        private async Task<ServicesResult<bool>> DeleteTaskMethod(string taskId)
        {
            if (string.IsNullOrEmpty(taskId))
                return ServicesResult<bool>.Failure("Invalid task ID.");

            return await _taskServices.DeleteAsync(taskId)
                ? ServicesResult<bool>.Success(true)
                : ServicesResult<bool>.Failure("Failed to delete task.");
        }
        #endregion

    }
}
