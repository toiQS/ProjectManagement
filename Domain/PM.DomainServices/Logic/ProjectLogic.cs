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
        private string _ownRole;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectLogic"/> class.
        /// </summary>
        public ProjectLogic(
            IApplicationUserServices applicationUserServices,
            IRoleApplicationUserInProjectServices roleApplicationUserServices,
            IProjectServices projectServices,
            IRoleInProjectServices roleInProjectServices,
            IStatusServices statusServices)
        {
            _applicationUserServices = applicationUserServices;
            _roleApplicationUserServices = roleApplicationUserServices;
            _projectServices = projectServices;
            _roleInProjectServices = roleInProjectServices;
            _statusServices = statusServices;

            // Initialize the owner's role.
            InitializeOwnerRole();

            // Update the status of all projects.
            UpdateAllProjectStatuses();
        }

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Initializes the owner's role by retrieving it from the database.
        /// </summary>
        private void InitializeOwnerRole()
        {
            ServicesResult<string> roleResult;
            do
            {
                roleResult = GetOwnerRoleId().GetAwaiter().GetResult();
            } while (!roleResult.Status);

            _ownRole = roleResult.Data;
        }

        /// <summary>
        /// Updates the statuses of all projects in the database.
        /// </summary>
        private void UpdateAllProjectStatuses()
        {
            ServicesResult<IEnumerable<Project>> projectList;
            do
            {
                projectList = _projectServices.GetAllAsync().GetAwaiter().GetResult();
            } while (!projectList.Status);

            foreach (var project in projectList.Data)
            {
                var result = UpdateStatusMethod(project.Id).GetAwaiter().GetResult();
                if (!result.Status)
                {
                    // Log failure (can integrate logging here if needed).
                    ServicesResult<bool>.Failure(result.Message);
                }
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


        #region AddProject
        /// <summary>
        /// Adds a new project if the user is authorized and no project with the same name exists for the user.
        /// </summary>
        /// <param name="userId">The ID of the user attempting to add the project.</param>
        /// <param name="addProject">The details of the project to be added.</param>
        /// <returns>A <see cref="ServicesResult{T}"/> indicating success or failure.</returns>
        public async Task<ServicesResult<bool>> Add(string userId, AddProject addProject)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || addProject == null)
                return ServicesResult<bool>.Failure("Invalid parameters.");

            // Check and retrieve user information
            var userResult = await CheckAndGetUserInfo(userId);
            if (!userResult.Status)
                return ServicesResult<bool>.Failure(userResult.Message);

            if (userResult.Data == null)
                return ServicesResult<bool>.Success(false);

            // Retrieve all roles associated with users
            var rolesResult = await _roleApplicationUserServices.GetAllAsync();
            if (!rolesResult.Status)
                return ServicesResult<bool>.Failure(rolesResult.Message);

            if (rolesResult.Data == null)
                return ServicesResult<bool>.Success(false);

            // Check if the user owns a project with the same name
            var ownedProjects = rolesResult.Data.Where(x => x.ApplicationUserId == userId).ToList();
            foreach (var role in ownedProjects)
            {
                var projectResult = await _projectServices.GetValueByPrimaryKeyAsync(role.ProjectId);
                if (!projectResult.Status || projectResult.Data == null)
                    continue;

                if (projectResult.Data.ProjectName == addProject.ProjectName)
                    return ServicesResult<bool>.Failure("A project with the same name already exists.");
            }

            // Add the new project
            var addResult = await AddMethod(userId, addProject);
            if (!addResult.Status)
                return ServicesResult<bool>.Failure(addResult.Message);

            return ServicesResult<bool>.Success(true);
        }
        #endregion

        #region AddMethod
        /// <summary>
        /// Internal method to create a new project and assign the user as the owner.
        /// </summary>
        /// <param name="userId">The ID of the user to be assigned as the owner.</param>
        /// <param name="addProject">The project details to be added.</param>
        /// <returns>A <see cref="ServicesResult{T}"/> indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> AddMethod(string userId, AddProject addProject)
        {
            // Generate unique IDs for the project and role
            var randomIdPart = new Random().Next(1000000, 9000000);
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            var project = new Project
            {
                Id = $"1001-{randomIdPart}-{timestamp}",
                ProjectName = addProject.ProjectName,
                CreateAt = DateTime.Now,
                StartAt = addProject.StartAt,
                EndAt = addProject.EndAt,
                ProjectDescription = addProject.ProjectDescription,
                IsAccessed = true,
                IsDeleted = false,
                IsDone = false,
                StatusId = DateTime.Now == addProject.StartAt
                    ? 3 // Ongoing
                    : (DateTime.Now < addProject.StartAt ? 2 : 1) // Upcoming or Overdue
            };

            // Save the project to the database
            var projectAddResult = await _projectServices.AddAsync(project);
            if (!projectAddResult.Status)
                return ServicesResult<bool>.Failure($"Failed to create the project. {projectAddResult.Message}");

            // Assign the user as the owner of the project
            var roleAssignment = new RoleApplicationUserInProject
            {
                Id = $"1002-{randomIdPart}-{timestamp}",
                ProjectId = project.Id,
                ApplicationUserId = userId,
                RoleInProjectId = _ownRole
            };

            var roleAddResult = await _roleApplicationUserServices.AddAsync(roleAssignment);
            if (!roleAddResult.Status)
                return ServicesResult<bool>.Failure("Failed to assign the user as project owner.");

            return ServicesResult<bool>.Success(true);
        }
        #endregion

        #region Update Project Information
        /// <summary>
        /// Updates project information such as name and description.
        /// </summary>
        public async Task<ServicesResult<bool>> UpdateInfo(string userId, string projectId, UpdateProject updateProject)
        {
            if (string.IsNullOrEmpty(projectId) || updateProject == null)
                return ServicesResult<bool>.Failure("Invalid input.");

            var userResult = await CheckAndGetUserInfo(userId);
            if (!userResult.Status) return ServicesResult<bool>.Failure(userResult.Message);
            if (userResult.Data == null) return ServicesResult<bool>.Success(false);

            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (project.Status == false || project.Data == null)
                return ServicesResult<bool>.Failure(project.Message);

            var rolesResult = await _roleApplicationUserServices.GetAllAsync();
            if (!rolesResult.Status) return ServicesResult<bool>.Failure(rolesResult.Message);

            var ownerRoles = rolesResult.Data
                .Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == _ownRole)
                .ToList();
            if (!ownerRoles.Any()) return ServicesResult<bool>.Success(false);

            if (project.Data.ProjectName == updateProject.ProjectName)
                return ServicesResult<bool>.Failure("A project with the same name already exists.");

            project.Data.ProjectName = updateProject.ProjectName;
            project.Data.ProjectDescription = updateProject.ProjectDescription;

            var result = await _projectServices.UpdateAsync(project.Data);
            return result.Status ? ServicesResult<bool>.Success(true) : ServicesResult<bool>.Failure(result.Message);
        }
        #endregion

        #region Update Project Flags (IsDeleted, IsAccessed, IsDone)
        /// <summary>
        /// Toggles the IsDeleted flag of a project.
        /// </summary>
        public async Task<ServicesResult<bool>> UpdateIsDeletedAsync(string userId, string projectId)
        {
            return await ToggleProjectFlagAsync(userId, projectId, project => project.IsDeleted = !project.IsDeleted);
        }

        /// <summary>
        /// Toggles the IsAccessed flag of a project.
        /// </summary>
        public async Task<ServicesResult<bool>> UpdateIsAccessedAsync(string userId, string projectId)
        {
            return await ToggleProjectFlagAsync(userId, projectId, project => project.IsAccessed = !project.IsAccessed);
        }

        /// <summary>
        /// Toggles the IsDone flag of a project.
        /// </summary>
        public async Task<ServicesResult<bool>> UpdateIsDoneAsync(string userId, string projectId)
        {
            return await ToggleProjectFlagAsync(userId, projectId, project => project.IsDone = !project.IsDone);
        }
        #endregion

        #region Update Project Status
        /// <summary>
        /// Updates the status of a project based on its completion and end date.
        /// </summary>
        public async Task<ServicesResult<bool>> UpdateProjectStatusAsync(string userId, string projectId)
        {
            var userResult = await CheckAndGetUserInfo(userId);
            if (!userResult.Status) return ServicesResult<bool>.Failure(userResult.Message);
            if (userResult.Data == null) return ServicesResult<bool>.Success(false);
            if (string.IsNullOrEmpty(projectId)) return ServicesResult<bool>.Failure("Invalid project ID.");

            var result = await UpdateStatusMethod(projectId);
            return result.Status ? ServicesResult<bool>.Success(true) : ServicesResult<bool>.Failure(result.Message);
        }

        /// <summary>
        /// Updates the status of a project.
        /// </summary>
        private async Task<ServicesResult<bool>> UpdateStatusMethod(string projectId)
        {
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (project.Status == false || project.Data == null)
                return ServicesResult<bool>.Failure(project.Message);

            if (!project.Data.IsDone && project.Data.EndAt < DateTime.Now)
                project.Data.StatusId = 6; // Ended but not done
            else if (!project.Data.IsDone && project.Data.EndAt == DateTime.Now)
                project.Data.StatusId = 5; // Ending today but not done
            else if (!project.Data.IsDone && project.Data.EndAt > DateTime.Now)
                project.Data.StatusId = 3; // Active but not done
            else if (project.Data.IsDone && project.Data.EndAt < DateTime.Now)
                project.Data.StatusId = 7; // Completed and ended
            else if (project.Data.IsDone && project.Data.EndAt == DateTime.Now)
                project.Data.StatusId = 5; // Completed, ending today
            else if (project.Data.IsDone && project.Data.EndAt > DateTime.Now)
                project.Data.StatusId = 4; // Completed, still ongoing

            var result = await _projectServices.UpdateAsync(project.Data);
            return result.Status ? ServicesResult<bool>.Success(true) : ServicesResult<bool>.Failure(result.Message);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Toggles a specific project flag (e.g., IsDeleted, IsAccessed, IsDone).
        /// </summary>
        private async Task<ServicesResult<bool>> ToggleProjectFlagAsync(string userId, string projectId, Action<Project> toggleAction)
        {
            var userResult = await CheckAndGetUserInfo(userId);
            if (!userResult.Status) return ServicesResult<bool>.Failure(userResult.Message);
            if (userResult.Data == null) return ServicesResult<bool>.Success(false);
            if (string.IsNullOrEmpty(projectId)) return ServicesResult<bool>.Failure("Invalid project ID.");

            var rolesResult = await _roleApplicationUserServices.GetAllAsync();
            if (!rolesResult.Status) return ServicesResult<bool>.Failure(rolesResult.Message);

            var ownerRoles = rolesResult.Data
                .Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == _ownRole)
                .ToList();
            if (!ownerRoles.Any()) return ServicesResult<bool>.Success(false);

            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (project.Status == false || project.Data == null) return ServicesResult<bool>.Failure(project.Message);

            toggleAction(project.Data);

            var result = await _projectServices.UpdateAsync(project.Data);
            return result.Status ? ServicesResult<bool>.Success(true) : ServicesResult<bool>.Failure(result.Message);
        }
        

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
