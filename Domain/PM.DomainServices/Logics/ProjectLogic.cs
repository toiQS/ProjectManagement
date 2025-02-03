using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using PM.Domain;
using PM.DomainServices.Models;
using PM.DomainServices.Models.projects;
using PM.DomainServices.UnitOfWorks;
using PM.Persistence.Context;

namespace PM.DomainServices.Logics
{
    public class ProjectLogic
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        #region Get Index Projects
        /// <summary>
        /// Retrieves a list of index projects asynchronously.
        /// </summary>
        /// <returns>A service result containing a collection of index projects.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetIndexProjectsAsync()
        {
            var response = new List<IndexProject>();

            // Fetch all projects from the repository
            var projects = await _unitOfWork.ProjectRepository.GetAllAsync();
            if (!projects.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(projects.Message);
            if (projects.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Success(response, projects.Message);

            foreach (var project in projects.Data)
            {
                // Retrieve positions associated with the project
                var positions = await _context.PositionInProject.Where(x => x.ProjectId == project.Id).ToListAsync();
                if (positions == null || positions.Count == 0) return ServicesResult<IEnumerable<IndexProject>>.Failure("No Position In Project");

                var memberProjects = new List<MemberProject>();

                // Fetch members for each position
                foreach (var position in positions)
                {
                    var members = await _context.MemberProject.Where(x => x.PositionInProjectId == position.Id).ToListAsync();
                    if (members != null && members.Any())
                    {
                        memberProjects.AddRange(members);
                    }
                }

                // Retrieve all roles in the project
                var roles = await _unitOfWork.RoleInProjectRepository.GetAllAsync();
                if (!roles.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(roles.Message);

                // Find the owner role
                var roleOwner = roles.Data.FirstOrDefault(x => x.RoleName == "Owner");
                if (roleOwner == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("Owner role not found in project");

                // Find the owner of the project
                var owner = memberProjects.FirstOrDefault(x => x.RoleInProjectId == roleOwner.Id);
                if (owner == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("Project owner not found");

                // Fetch owner details
                var user = await _unitOfWork.ApplicationUserRepository.GetValueByPrimaryKey(owner.ApplicationUserId);
                if (!user.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(user.Message);

                // Create project index entry
                var indexProject = new IndexProject()
                {
                    OwnerAvata = user.Data.PathImage ?? "Error",
                    OwnerName = user.Data.UserName ?? "Error",
                    ProjectName = project.ProjectName,
                    ProjectId = project.Id,
                };

                response.Add(indexProject);
            }

            return ServicesResult<IEnumerable<IndexProject>>.Success(response, string.Empty);
        }
        #endregion
        #region Get Project List User Has Joined
        /// <summary>
        /// Retrieves a list of projects that a user has joined.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A service result containing a collection of index projects.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectListUserHasJoined(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return ServicesResult<IEnumerable<IndexProject>>.Failure("User ID is required");

            // Get projects the user has joined
            var projectJoined = await _context.MemberProject.Where(x => x.ApplicationUserId == userId).ToListAsync();
            if (projectJoined == null || !projectJoined.Any()) return ServicesResult<IEnumerable<IndexProject>>.Failure("User has not joined any projects");

            // Find user's positions in projects
            var positionList = new List<PositionInProject>();
            foreach (var index in projectJoined)
            {
                var position = await _unitOfWork.PositionInProjectRepository.GetValueByPrimaryKey(index.PositionInProjectId);
                if (!position.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(position.Message);
                positionList.Add(position.Data);
            }

            // Retrieve projects
            var projectList = new List<Project>();
            foreach (var index in positionList)
            {
                var project = await _unitOfWork.ProjectRepository.GetValueByPrimaryKey(index.ProjectId);
                if (!project.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(project.Message);
                projectList.Add(project.Data);
            }

            var response = new List<IndexProject>();

            foreach (var project in projectList)
            {
                var positionsInProject = await _context.PositionInProject.Where(x => x.ProjectId == project.Id).ToListAsync();
                if (positionsInProject == null || !positionsInProject.Any()) return ServicesResult<IEnumerable<IndexProject>>.Failure("No positions found in project");

                var memberProjects = new List<MemberProject>();

                foreach (var position in positionsInProject)
                {
                    var members = await _context.MemberProject.Where(x => x.PositionInProjectId == position.Id).ToListAsync();
                    if (members != null && members.Any())
                    {
                        memberProjects.AddRange(members);
                    }
                }

                var roles = await _unitOfWork.RoleInProjectRepository.GetAllAsync();
                if (!roles.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(roles.Message);

                var roleOwner = roles.Data.FirstOrDefault(x => x.RoleName == "Owner");
                if (roleOwner == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("No owner found in project");

                var owner = memberProjects.FirstOrDefault(x => x.RoleInProjectId == roleOwner.Id);
                if (owner == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("Project owner not found");

                var user = await _unitOfWork.ApplicationUserRepository.GetValueByPrimaryKey(owner.ApplicationUserId);
                if (!user.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(user.Message);

                var indexProject = new IndexProject()
                {
                    OwnerAvata = user.Data.PathImage ?? "Error",
                    OwnerName = user.Data.UserName ?? "Error",
                    ProjectName = project.ProjectName,
                    ProjectId = project.Id,
                };

                response.Add(indexProject);
            }

            return ServicesResult<IEnumerable<IndexProject>>.Success(response, string.Empty);
        }
        #endregion

        #region Get List of Projects User Owns
        /// <summary>
        /// Retrieves a list of projects where the user is the owner.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A service result containing a collection of index projects.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetListProjectUserHasOwner(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return ServicesResult<IEnumerable<IndexProject>>.Failure("User ID is required");

            // Retrieve all roles and find the owner role
            var roles = await _unitOfWork.RoleInProjectRepository.GetAllAsync();
            if (!roles.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(roles.Message);
            var roleOwner = roles.Data.FirstOrDefault(x => x.RoleName == "Owner");
            if (roleOwner == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("Owner role not found");

            // Get projects where the user is the owner
            var projectJoined = await _context.MemberProject.Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == roleOwner.Id).ToListAsync();
            if (projectJoined == null || !projectJoined.Any()) return ServicesResult<IEnumerable<IndexProject>>.Failure("User has not joined any projects as an owner");

            // Retrieve user details
            var user = await _unitOfWork.ApplicationUserRepository.GetValueByPrimaryKey(userId);
            if (!user.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(user.Message);

            // Find the user's position in each project
            var positionList = new List<PositionInProject>();
            foreach (var index in projectJoined)
            {
                var position = await _unitOfWork.PositionInProjectRepository.GetValueByPrimaryKey(index.PositionInProjectId);
                if (!position.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(position.Message);
                positionList.Add(position.Data);
            }

            // Retrieve project details
            var projectList = new List<IndexProject>();
            foreach (var position in positionList)
            {
                var project = await _unitOfWork.ProjectRepository.GetValueByPrimaryKey(position.ProjectId);
                if (!project.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(project.Message);

                var indexProject = new IndexProject()
                {
                    OwnerName = user.Data.UserName ?? "Error",
                    ProjectName = project.Data.ProjectName ?? "Error",
                    OwnerAvata = user.Data.PathImage ?? "Error",
                    ProjectId = position.ProjectId,
                };

                projectList.Add(indexProject);
            }

            return ServicesResult<IEnumerable<IndexProject>>.Success(projectList, string.Empty);
        }
        #endregion

    }
}
