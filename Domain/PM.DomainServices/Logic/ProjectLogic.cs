using Microsoft.AspNetCore.Mvc.ModelBinding;
using PM.Domain;
using PM.DomainServices.ILogic;
using PM.DomainServices.Models;
using PM.DomainServices.Models.projects;
using PM.DomainServices.Models.users;
using PM.Persistence.IServices;

namespace PM.DomainServices.Logic
{
    public class ProjectLogic
    {
        #region Fields and Constructor

        private readonly IApplicationUserServices _applicationUserServices;
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserServices;
        private readonly IProjectServices _projectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        private readonly IStatusServices _statusServices;
        private readonly IMemberLogic _memberLogic;
        private readonly IPlanLogic _planLogic;
        private readonly IPositionLogic _positionLogic;
        private string _ownerRole;

        public ProjectLogic(
            IApplicationUserServices applicationUserServices,
            IRoleApplicationUserInProjectServices roleApplicationUserServices,
            IProjectServices projectServices,
            IRoleInProjectServices roleInProjectServices,
            IStatusServices statusServices,
            IMemberLogic memberLogic,
            IPlanLogic planLogic,
            IPositionLogic positionLogic)
        {
            _applicationUserServices = applicationUserServices;
            _roleApplicationUserServices = roleApplicationUserServices;
            _projectServices = projectServices;
            _roleInProjectServices = roleInProjectServices;
            _statusServices = statusServices;
            _memberLogic = memberLogic;
            _planLogic = planLogic;
            _positionLogic = positionLogic;

            InitializeOwnerRole();
            InitializeAutoUpdateAllProjects();
        }

        #endregion

        #region Private Methods - Helpers

        /// <summary>
        /// Validates and retrieves application user details by ID or email.
        /// </summary>
        private async Task<ServicesResult<DetailAppUser>> CheckAndGetAppUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<DetailAppUser>.Failure("User ID cannot be null or empty.");

            var info = await _applicationUserServices.GetAppUserByIdOrEmail(userId);

            if (info.Data == null || !info.Status)
                return ServicesResult<DetailAppUser>.Failure(info.Message);

            return ServicesResult<DetailAppUser>.Success(info.Data);
        }

        /// <summary>
        /// Retrieves the ID of the "Owner" role in projects.
        /// </summary>
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

            _ownerRole = roleResult.Data;
        }

        /// <summary>
        /// Initializes automatic updates for all projects' statuses.
        /// </summary>
        private async void InitializeAutoUpdateAllProjects()
        {
            var projectResult = await GetAllProjects();
            if (!projectResult.Status || projectResult.Data == null) return;

            foreach (var project in projectResult.Data)
            {
                var update = UpdateStatusMethod(project.Id).GetAwaiter().GetResult();
                if (!update.Status) return;
            }
        }

        #endregion

        #region Private Methods - Core Logic

        /// <summary>
        /// Retrieves all projects.
        /// </summary>
        private async Task<ServicesResult<IEnumerable<Project>>> GetAllProjects()
        {
            var projects = await _projectServices.GetAllAsync();
            if (!projects.Status)
                return ServicesResult<IEnumerable<Project>>.Failure(projects.Message);

            return ServicesResult<IEnumerable<Project>>.Success(projects.Data);
        }

        /// <summary>
        /// Checks and retrieves detailed project information.
        /// </summary>
        private async Task<ServicesResult<Project>> CheckAndGetDetailProject(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return ServicesResult<Project>.Failure("Project ID cannot be null or empty.");

            var detailProject = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (!detailProject.Status || detailProject.Data == null)
                return ServicesResult<Project>.Failure(detailProject.Message);

            return ServicesResult<Project>.Success(detailProject.Data);
        }

        /// <summary>
        /// Updates the status of a project based on its completion and end date.
        /// </summary>
        private async Task<ServicesResult<bool>> UpdateStatusMethod(string projectId)
        {
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (!project.Status || project.Data == null)
                return ServicesResult<bool>.Failure(project.Message);

            var data = project.Data;

            if (!data.IsDone && data.EndAt < DateTime.Now)
                data.StatusId = 6; // Ended but not done
            else if (!data.IsDone && data.EndAt == DateTime.Now)
                data.StatusId = 5; // Ending today but not done
            else if (!data.IsDone && data.EndAt > DateTime.Now)
                data.StatusId = 3; // Active but not done
            else if (data.IsDone && data.EndAt < DateTime.Now)
                data.StatusId = 7; // Completed and ended
            else if (data.IsDone && data.EndAt == DateTime.Now)
                data.StatusId = 5; // Completed, ending today
            else if (data.IsDone && data.EndAt > DateTime.Now)
                data.StatusId = 4; // Completed, still ongoing

            var result = await _projectServices.UpdateAsync(data);
            return result.Status
                ? ServicesResult<bool>.Success(true)
                : ServicesResult<bool>.Failure(result.Message);
        }

        /// <summary>
        /// Adds a new project and assigns the owner role to the user.
        /// </summary>
        private async Task<ServicesResult<bool>> AddProject(string userId, AddProject addProject)
        {
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

            var projectAddResult = await _projectServices.AddAsync(project);
            if (!projectAddResult.Status)
                return ServicesResult<bool>.Failure($"Failed to create the project. {projectAddResult.Message}");

            var roleAssignment = new RoleApplicationUserInProject
            {
                Id = $"1002-{randomIdPart}-{timestamp}",
                ProjectId = project.Id,
                ApplicationUserId = userId,
                RoleInProjectId = _ownerRole
            };

            var roleAddResult = await _roleApplicationUserServices.AddAsync(roleAssignment);
            if (!roleAddResult.Status)
                return ServicesResult<bool>.Failure("Failed to assign the user as project owner.");

            return ServicesResult<bool>.Success(true);
        }

        /// <summary>
        /// Updates project information.
        /// </summary>
        private async Task<ServicesResult<bool>> UpdateProjectInfo(string projectId, UpdateProject updateProject)
        {
            var projectResult = await CheckAndGetDetailProject(projectId);
            if (!projectResult.Status || projectResult.Data == null)
                return ServicesResult<bool>.Failure(projectResult.Message);

            var data = projectResult.Data;
            data.ProjectName = updateProject.ProjectName;
            data.ProjectDescription = updateProject.ProjectDescription;

            var updateResult = await _projectServices.UpdateAsync(data);
            return updateResult.Status
                ? ServicesResult<bool>.Success(true)
                : ServicesResult<bool>.Failure(updateResult.Message);
        }

        private async Task<ServicesResult<bool>> CheckUserHasJoinedProject(string userId, string projectId)
        {
            var userResult = await CheckAndGetAppUser(userId);
            if (!userResult.Status) return ServicesResult<bool>.Failure(userResult.Message);
            if (userResult.Data == null) return ServicesResult<bool>.Success( true);
            var project = await CheckAndGetDetailProject(projectId);
            if (!userResult.Status) return ServicesResult<bool>.Failure(project.Message);
            if (userResult.Data == null) return ServicesResult<bool>.Success(true);
            var getRoleResult = await _roleApplicationUserServices.GetAllAsync();
            if(!getRoleResult.Status || getRoleResult.Data == null) return ServicesResult<bool>.Failure(getRoleResult.Message);
            return ServicesResult<bool>.Success(true);
        }

        #endregion

        #region Public Methods - Project Management



        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectListUserHasJoined(string userId)
        {
            var userResult = await CheckAndGetAppUser(userId);
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
                var projectResult = await CheckAndGetDetailProject(role.ProjectId);
                if(projectResult.Status == false || projectResult.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Failure(projectResult.Message);
                if (projectResult.Data.IsDeleted == true) continue;

                var ownerRole = rolesResult.Data.FirstOrDefault(x => x.RoleInProjectId == _ownerRole);
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

        #endregion
    }
}
