using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using PM.Domain;
using PM.DomainServices.ILogic;
using PM.DomainServices.OutDataDTOs;
using PM.DomainServices.Shared;
using PM.Persistence.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PM.DomainServices.Logic
{
    public class ProjectLogic
    {
        private readonly IApplicationUserServices _applicationUserServices;
        private readonly IProjectServices _projectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        private readonly IPositionInProjectServices _positionInProjectServices;
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
            IApplicationUserServices applicationUserServices)
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

        #region delete project
        public async Task<ServicesResult<bool>> DeleteProject(string userId, string projectId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId)) return ServicesResult<bool>.Failure("");
            var roles = await _roleInProjectServices.GetAllAsync();
            var ownerRole = roles.FirstOrDefault(x => x.RoleName == "Owner");
            if (ownerRole == null) return ServicesResult<bool>.Failure("Owner role not found.");
            var roleUserInProjects = await _roleApplicationUserInProjectServices.GetAllAsync();
            var userProject = roleUserInProjects.Where(x => x.ProjectId == projectId && x.ApplicationUserId == userId && x.RoleInProjectId == ownerRole.Id).FirstOrDefault();
            if (userProject == null) return ServicesResult<bool>.Failure("");
            // delete all component of project
        }

        #endregion
    }
}
