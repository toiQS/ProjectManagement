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
        private DetailAppUser DetailAppUser { get; set; } = new DetailAppUser();
        private readonly IApplicationUserServices _applicationUserServices;
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserServices;
        private readonly IProjectServices _projectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        private readonly IStatusServices _statusServices;
        private readonly IMemberLogic _memberLogic;
        private readonly IPlanLogic _planLogic;
        private readonly IPositionLogic _positionLogic;
        private string _ownerRole = string.Empty;

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
                return ServicesResult<DetailAppUser>.Success(null, "User not found.");
            }

            // Initialize the retrieved user for this logic
            DetailAppUser = userResult.Data;

            // Return the successful result with the retrieved user data
            return ServicesResult<DetailAppUser>.Success(userResult.Data,string.Empty);
        }
        #endregion


        #region Get list projects user has joined
        /// <summary>
        /// Retrieves a list of projects that the currently logged-in user has joined.
        /// </summary>
        /// <returns>A service result containing a list of projects the user has joined.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetListProjectsUserHasJoined()
        {
            var result = new List<IndexProject>();

            // Retrieve all members from the role-application-user service
            var getMembers = await _roleApplicationUserServices.GetAllAsync();
            if (!getMembers.Status)
                return ServicesResult<IEnumerable<IndexProject>>.Failure(getMembers.Message);

            if (getMembers.Data == null)
                return ServicesResult<IEnumerable<IndexProject>>.Success(result, "Not found any members in database");

            // Filter projects where the current user is a member
            var projectJoinedResult = getMembers.Data.Where(x => x.ApplicationUserId == DetailAppUser.UserId).ToList();
            if (!projectJoinedResult.Any())
                return ServicesResult<IEnumerable<IndexProject>>.Success(result, "Not found any project of user has joined");

            foreach (var item in projectJoinedResult)
            {
                // Retrieve project details
                var project = await _projectServices.GetValueByPrimaryKeyAsync(item.ProjectId);
                if (project.Data == null || !project.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(project.Message);

                if (project.Data.IsDeleted)
                    continue;

                // Find the owner of the project
                var ownerRoleResult = getMembers.Data.FirstOrDefault(x => x.RoleInProjectId == _ownerRole && x.ProjectId == item.ProjectId);
                if (ownerRoleResult == null)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure($"Not found owner of this project {project.Data.ProjectName}");

                var owner = await CheckAndGetUser(ownerRoleResult.ApplicationUserId);
                if (!owner.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure("Can't find info of owner this project");
                if (owner.Data == null)
                    return ServicesResult<IEnumerable<IndexProject>>.Success(result, $"Can't get info of owner this project ,{project.Data.ProjectName}"); 
                // Add the project to the result list
                var itemProject = new IndexProject
                {
                    ProjectId = project.Data.Id,
                    ProjectName = project.Data.ProjectName,
                    OwnerName = owner.Data.UserName,
                    OwnerAvata = owner.Data.Avata,
                };
                result.Add(itemProject);
            }

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
            var result = new List<IndexProject>();

            // Retrieve all member-role mappings from the role-application-user service
            var getMembers = await _roleApplicationUserServices.GetAllAsync();
            if (!getMembers.Status)
                return ServicesResult<IEnumerable<IndexProject>>.Failure(getMembers.Message);

            if (getMembers.Data == null)
                return ServicesResult<IEnumerable<IndexProject>>.Success(result, "Can't get any member in database");

            // Filter roles where the user is an owner in any project
            var ownerRoles = getMembers.Data
                .Where(x => x.ApplicationUserId == DetailAppUser.UserId && x.RoleInProjectId == _ownerRole)
                .ToList();

            if (!ownerRoles.Any())
                return ServicesResult<IEnumerable<IndexProject>>.Success(result, "Can't find role user of this project");

            var projects = new List<IndexProject>();

            foreach (var role in ownerRoles)
            {
                // Retrieve project details
                var projectResult = await _projectServices.GetValueByPrimaryKeyAsync(role.ProjectId);
                if (projectResult.Data == null || !projectResult.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure($"{projectResult.Message}, can't get project {role.ProjectId}");
                if (projectResult.Data.IsDeleted)
                    continue;

                // Add the project to the result list
                projects.Add(new IndexProject
                {
                    OwnerAvata = DetailAppUser.Avata,
                    OwnerName = DetailAppUser.UserName,
                    ProjectName = projectResult.Data.ProjectName,
                    ProjectId = projectResult.Data.Id,
                });
            }

            return ServicesResult<IEnumerable<IndexProject>>.Success(projects, string.Empty);
        }
        #endregion


        #region get detail project user has joined
        public async Task<ServicesResult<DetailProject>> GetProjectDetailProjectHasJoined(string projectId)
        {
            var result = new DetailProject();

            if (string.IsNullOrEmpty(projectId))
                return ServicesResult<DetailProject>.Failure("Project ID cannot be null or empty.");

            // Retrieve roles related to the user
            var getMembers = await _roleApplicationUserServices.GetAllAsync();
            if (!getMembers.Status)
                return ServicesResult<DetailProject>.Failure(getMembers.Message);

            if (getMembers.Data == null)
                return ServicesResult<DetailProject>.Success(result,"Can't find any member in this database");

            // Check if the user is associated with the specified project
            var userRolesInProject = getMembers.Data.Where(x => x.ApplicationUserId == DetailAppUser.UserId && x.ProjectId == projectId).ToList();
            if (!userRolesInProject.Any())
                return ServicesResult<DetailProject>.Success(result, "can't find any proejct that user has joined ");

            // Retrieve project information
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (!project.Status)
                return ServicesResult<DetailProject>.Failure(project.Message);

            if (project.Data == null)
                return ServicesResult<DetailProject>.Success(result,"Project not found.");

            // Count the number of members in the project
            var memberCount = getMembers.Data.Count(x => x.ProjectId == projectId);

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
            result = new DetailProject
            {
                OwnerName = DetailAppUser.UserName,
                OwnerAvata = DetailAppUser.Avata,
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

            return ServicesResult<DetailProject>.Success(result, string.Empty);
        }
        #endregion

        #region add project
        public async Task<ServicesResult<bool>> Add( AddProject addProject)
        {
            // Retrieve all roles associated with users
            var getMembers = await _roleApplicationUserServices.GetAllAsync();
            if (!getMembers.Status)
                return ServicesResult<bool>.Failure(getMembers.Message);

            if (getMembers.Data == null)
                return ServicesResult<bool>.Success(false,"Can't find any member in database");

            // Check if the user owns a project with the same name
            var ownedProjects = getMembers.Data.Where(x => x.ApplicationUserId == DetailAppUser.UserId &&x.RoleInProjectId == _ownerRole).ToList();

            if(ownedProjects.Any())
            {
                foreach (var role in ownedProjects)
                {
                    var projectResult = await _projectServices.GetValueByPrimaryKeyAsync(role.ProjectId);
                    if (!projectResult.Status || projectResult.Data == null)
                        continue;

                    if (projectResult.Data.ProjectName == addProject.ProjectName)
                        return ServicesResult<bool>.Failure("A project with the same name already exists.");
                }
            }

            // Add the new project
            var addResult = await AddMethod( addProject);
            if (!addResult.Status)
                return ServicesResult<bool>.Failure(addResult.Message);

            return ServicesResult<bool>.Success(true, string.Empty);
        }
        #endregion

        #region Update Project Information
        /// <summary>
        /// Updates project information such as name and description.
        /// </summary>
        public async Task<ServicesResult<bool>> UpdateInfo(string projectId, UpdateProject updateProject)
        {
            if (string.IsNullOrEmpty(projectId) || updateProject == null)
                return ServicesResult<bool>.Failure("Invalid input.");

            
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (project.Status == false || project.Data == null)
                return ServicesResult<bool>.Failure(project.Message);

            var rolesResult = await _roleApplicationUserServices.GetAllAsync();
            if (!rolesResult.Status) return ServicesResult<bool>.Failure(rolesResult.Message);
            if (rolesResult.Data == null) return ServicesResult<bool>.Success(false, "Can't find any member in database");
            var ownerRoles = rolesResult.Data
                .Where(x => x.ApplicationUserId == DetailAppUser.UserId && x.RoleInProjectId == _ownerRole)
                .ToList();
            if (!ownerRoles.Any()) return ServicesResult<bool>.Success(false,"can't get any project that user are a owner");

            if (project.Data.ProjectName == updateProject.ProjectName)
                return ServicesResult<bool>.Failure("A project with the same name already exists.");

            project.Data.ProjectName = updateProject.ProjectName;
            project.Data.ProjectDescription = updateProject.ProjectDescription;

            var result = await _projectServices.UpdateAsync(project.Data);
            return result.Status ? ServicesResult<bool>.Success(true,string.Empty) : ServicesResult<bool>.Failure(result.Message);
        }
        #endregion



        #region Private Method Support

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
        /// Toggles a specific flag on a project based on a provided action.
        /// </summary>
        /// <param name="projectId">The ID of the project to update.</param>
        /// <param name="toggleAction">An action to apply changes to the project.</param>
        /// <returns>A service result indicating success or failure of the operation.</returns>
        private async Task<ServicesResult<bool>> ToggleProjectFlagAsync(string projectId, Action<Project> toggleAction)
        {
            // Validate project ID
            if (string.IsNullOrEmpty(projectId))
                return ServicesResult<bool>.Failure("Invalid project ID.");

            // Retrieve all members from the database
            var getMembers = await _roleApplicationUserServices.GetAllAsync();
            if (!getMembers.Status)
                return ServicesResult<bool>.Failure(getMembers.Message);

            if (getMembers.Data == null)
                return ServicesResult<bool>.Failure("No members found in the database.");

            // Check if the current user is an owner in the project
            var ownerRoles = getMembers.Data
                .Where(x => x.ApplicationUserId == DetailAppUser.UserId && x.RoleInProjectId == _ownerRole)
                .ToList();

            if (!ownerRoles.Any())
                return ServicesResult<bool>.Failure("No projects found where you are the owner.");

            // Retrieve the project by its ID
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (!project.Status || project.Data == null)
                return ServicesResult<bool>.Failure(project.Message);

            // Apply the toggle action to the project
            toggleAction(project.Data);

            // Update the project in the database
            var result = await _projectServices.UpdateAsync(project.Data);
            return result.Status
                ? ServicesResult<bool>.Success(true, string.Empty)
                : ServicesResult<bool>.Failure(result.Message);
        }

        /// <summary>
        /// Adds a new project and assigns the current user as its owner.
        /// </summary>
        /// <param name="addProject">The project details to be added.</param>
        /// <returns>A service result indicating success or failure of the operation.</returns>
        private async Task<ServicesResult<bool>> AddMethod(AddProject addProject)
        {
            // Generate unique IDs for the project and role
            var randomIdPart = new Random().Next(1000000, 9000000);
            var timestamp = DateTime.Now.ToString("ddMMyyyy");

            // Create a new project instance
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
                ApplicationUserId = DetailAppUser.UserId,
                RoleInProjectId = _ownerRole
            };

            var roleAddResult = await _roleApplicationUserServices.AddAsync(roleAssignment);
            if (!roleAddResult.Status)
                return ServicesResult<bool>.Failure("Failed to assign the user as project owner.");

            return ServicesResult<bool>.Success(true, string.Empty);
        }

        #endregion
    }
}
