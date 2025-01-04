using Microsoft.AspNetCore.Hosting;
using PM.Domain;
using PM.DomainServices.Models;
using PM.DomainServices.Models.projects;
using PM.DomainServices.Models.users;
using PM.Persistence.IServices;
using System.Data;

namespace PM.DomainServices.Logic
{
    /// <summary>
    /// Provides logic for handling project-related operations, such as retrieving projects joined or owned by a user.
    /// </summary>
    public class ProjectLogic
    {
        #region Fields and Constructor

        private readonly IApplicationUserServices _applicationUserServices;
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserServices;
        private readonly IProjectServices _projectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        private readonly IStatusServices _statusServices;
        private static string _ownRole;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectLogic"/> class.
        /// </summary>
        public ProjectLogic(
            IApplicationUserServices applicationUserServices,
            IRoleApplicationUserInProjectServices roleApplicationUserServices,
            IProjectServices projectServices,
            IRoleInProjectServices roleInProjectServices)
        {
            _applicationUserServices = applicationUserServices;
            _roleApplicationUserServices = roleApplicationUserServices;
            _projectServices = projectServices;
            _roleInProjectServices = roleInProjectServices;

            // Set the owner's role on initialization.
            var roleResult = GetOwnerRoleId().GetAwaiter().GetResult();
            if (roleResult.Status)
            {
                _ownRole = roleResult.Data;
            }
        }

        #endregion

        #region Methods for Retrieving Projects

        /// <summary>
        /// Retrieves the list of projects a user has joined.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A <see cref="ServicesResult{T}"/> containing a list of <see cref="IndexProject"/> objects.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectListUserHasJoined(string userId)
        {
            var userResult = await CheckAndGetUserInfo(userId);
            if (!userResult.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(userResult.Message);
            if (userResult.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Success(null);

            var rolesResult = await _roleApplicationUserServices.GetAllAsync();
            if (!rolesResult.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(rolesResult.Message);
            if (rolesResult.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Success(null);

            var userRoles = rolesResult.Data.Where(x => x.ApplicationUserId == userId).ToList();
            if (!userRoles.Any()) return ServicesResult<IEnumerable<IndexProject>>.Success(null);

            var projects = new List<IndexProject>();
            foreach (var role in userRoles)
            {
                var projectResult = await _projectServices.GetValueByPrimaryKeyAsync(role.ProjectId);
                if (projectResult.Data == null || !projectResult.Status || projectResult.Data.IsDeleted)
                    continue;

                var ownerRole = rolesResult.Data.FirstOrDefault(x => x.RoleInProjectId == _ownRole);
                if (ownerRole == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("Can't find owner of this project");

                var ownerResult = await _applicationUserServices.GetAppUserByIdOrEmail(ownerRole.ApplicationUserId);
                if (!ownerResult.Status || ownerResult.Data == null) continue;

                projects.Add(new IndexProject
                {
                    OwnerName = ownerResult.Data.UserName,
                    OwnerAvata = ownerResult.Data.Avata,
                    ProjectName = projectResult.Data.ProjectName,
                    ProjectId = projectResult.Data.Id,
                });
            }

            return ServicesResult<IEnumerable<IndexProject>>.Success(projects);
        }

        /// <summary>
        /// Retrieves the list of projects where the specified user is the owner.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A <see cref="ServicesResult{T}"/> containing a list of <see cref="IndexProject"/> objects.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectListUserHasOwner(string userId)
        {
            var userResult = await CheckAndGetUserInfo(userId);
            if (!userResult.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(userResult.Message);
            if (userResult.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Success(null);

            var rolesResult = await _roleApplicationUserServices.GetAllAsync();
            if (!rolesResult.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(rolesResult.Message);
            if (rolesResult.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Success(null);

            var ownerRoles = rolesResult.Data.Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == _ownRole).ToList();
            if (!ownerRoles.Any()) return ServicesResult<IEnumerable<IndexProject>>.Success(null);

            var projects = new List<IndexProject>();
            foreach (var role in ownerRoles)
            {
                var projectResult = await _projectServices.GetValueByPrimaryKeyAsync(role.ProjectId);
                if (projectResult.Data == null || !projectResult.Status || projectResult.Data.IsDeleted)
                    continue;

                projects.Add(new IndexProject
                {
                    OwnerAvata = userResult.Data.Avata,
                    OwnerName = userResult.Data.UserName,
                    ProjectName = projectResult.Data.ProjectName,
                    ProjectId = projectResult.Data.Id,
                });
            }

            return ServicesResult<IEnumerable<IndexProject>>.Success(projects);
        }

        #endregion

        #region GetProjectDetailProjectHasJoined
        /// <summary>
        /// Retrieves detailed information about a project the user has joined.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the project details.</param>
        /// <param name="projectId">The ID of the project to retrieve details for.</param>
        /// <returns>A <see cref="ServicesResult{T}"/> containing the detailed project information.</returns>
        public async Task<ServicesResult<DetailProject>> GetProjectDetailProjectHasJoined(string userId, string projectId)
        {
            // Validate user and project input
            var userResult = await CheckAndGetUserInfo(userId);
            if (!userResult.Status)
                return ServicesResult<DetailProject>.Failure(userResult.Message);

            if (userResult.Data == null)
                return ServicesResult<DetailProject>.Success(null);

            if (string.IsNullOrEmpty(projectId))
                return ServicesResult<DetailProject>.Failure("Project ID cannot be null or empty.");

            // Retrieve roles related to the user
            var rolesResult = await _roleApplicationUserServices.GetAllAsync();
            if (!rolesResult.Status)
                return ServicesResult<DetailProject>.Failure(rolesResult.Message);

            if (rolesResult.Data == null)
                return ServicesResult<DetailProject>.Success(null);

            // Check if the user is associated with the specified project
            var userRolesInProject = rolesResult.Data.Where(x => x.ApplicationUserId == userId && x.ProjectId == projectId).ToList();
            if (!userRolesInProject.Any())
                return ServicesResult<DetailProject>.Success(null);

            // Retrieve project information
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (!project.Status)
                return ServicesResult<DetailProject>.Failure(project.Message);

            if (project.Data == null)
                return ServicesResult<DetailProject>.Failure("Project not found.");

            // Count the number of members in the project
            var memberCount = rolesResult.Data.Count(x => x.ProjectId == projectId);

            // Retrieve the status of the project
            var statusResult = await _statusServices.GetAllAsync();
            if (!statusResult.Status)
                return ServicesResult<DetailProject>.Failure(statusResult.Message);

            if (statusResult.Data == null)
                return ServicesResult<DetailProject>.Failure("No statuses found.");

            var status = statusResult.Data.FirstOrDefault(x => x.Id == project.Data.StatusId);
            if (status == null)
                return ServicesResult<DetailProject>.Failure("Status for the project could not be found.");

            // Construct the project detail result
            var result = new DetailProject
            {
                OwnerName = userResult.Data.UserName,
                OwnerAvata = userResult.Data.Avata,
                ProjectId = projectId,
                ProjectName = project.Data.ProjectName,
                ProjectDescription = project.Data.ProjectDescription,
                StartAt = project.Data.StartAt,
                EndAt = project.Data.EndAt,
                CreateAt = project.Data.CreateAt,
                IsAccessed = project.Data.IsAccessed,
                IsDeleted = project.Data.IsDeleted,
                IsDone = project.Data.IsDone,
                QuantityMember = memberCount,
                Members = new List<Models.members.IndexMember>(), // Placeholder for members
                Plans = new List<Models.plans.IndexPlan>()        // Placeholder for plans
            };

            return ServicesResult<DetailProject>.Success(result);
        }
        #endregion





        #region Helper Methods

        /// <summary>
        /// Retrieves the user information and validates the input.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A <see cref="ServicesResult{T}"/> containing the user details.</returns>
        private async Task<ServicesResult<DetailAppUser>> CheckAndGetUserInfo(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<DetailAppUser>.Failure("User ID cannot be null or empty.");

            var userResult = await _applicationUserServices.GetAppUserByIdOrEmail(userId);
            if (!userResult.Status || userResult.Data == null)
                return ServicesResult<DetailAppUser>.Failure(userResult.Message);

            return ServicesResult<DetailAppUser>.Success(userResult.Data);
        }

        /// <summary>
        /// Retrieves the ID of the "Owner" role in projects.
        /// </summary>
        /// <returns>A <see cref="ServicesResult{T}"/> containing the ID of the owner role if found.</returns>
        private async Task<ServicesResult<string>> GetOwnerRoleId()
        {
            var rolesResult = await _roleInProjectServices.GetAllAsync();
            if (!rolesResult.Status)
                return ServicesResult<string>.Failure(rolesResult.Message);

            var ownerRoleId = rolesResult.Data?.FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            return ownerRoleId != null
                ? ServicesResult<string>.Success(ownerRoleId)
                : ServicesResult<string>.Failure("Owner role not found.");
        }

        #endregion
    }
}
