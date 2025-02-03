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

        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectListUserHasJoined(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return ServicesResult<IEnumerable<IndexProject>>.Failure("id is request");
            var projectJoined = await _context.MemberProject.Where(x => x.ApplicationUserId == userId).ToListAsync();
            if (projectJoined == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("User is not join any projects");
            
            /// find position of user in each project
            var positionList = new List<PositionInProject>();
            foreach (var index in projectJoined)
            {
                var position = await _unitOfWork.PositionInProjectRepository.GetValueByPrimaryKey(index.PositionInProjectId);
                if(position.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(position.Message);
                positionList.Add(position.Data);
            }
            var projectList = new List<Project>();
            foreach (var index in positionList)
            {
                var project = await _unitOfWork.ProjectRepository.GetValueByPrimaryKey(index.ProjectId);
                if(project.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(project.Message);
                projectList.Add(project.Data);
            }
            var response = new List<IndexProject>();


            foreach (var project in projectList)
            {
                var positionInProject = await _context.PositionInProject.Where(x => x.ProjectId == project.Id).ToListAsync();
                if (positionInProject == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("No Position In Project");
                var memberProject = new LinkedList<List<MemberProject>>();
                foreach (var indexPosition in positionInProject)
                {
                    var member = await _context.MemberProject.Where(x => x.PositionInProjectId == indexPosition.Id).ToListAsync();
                    if (member == null) continue;
                    memberProject.AddLast(member);
                }
                var roles = await _unitOfWork.RoleInProjectRepository.GetAllAsync();
                if (roles.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(roles.Message);
                var roleOwner = roles.Data.Where(x => x.RoleName == "Owner").FirstOrDefault();
                if (roleOwner == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("No find owner in this project");
                var ownProject = new MemberProject();
                foreach (var member in memberProject)
                {
                    if (ownProject == null) continue;
                    ownProject = _context.MemberProject.Where(x => x.RoleInProjectId == roleOwner.Id).FirstOrDefault();
                }
                var user = await _unitOfWork.ApplicationUserRepository.GetValueByPrimaryKey(ownProject.ApplicationUserId);
                if (user.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(user.Message);
                var index = new IndexProject()
                {
                    OwnerAvata = user.Data.PathImage ?? "Error",
                    OwnerName = user.Data.UserName ?? "Error",
                    ProjectName = project.ProjectName,
                    ProjectId = project.Id,
                };
                response.Add(index);
            }
            return ServicesResult<IEnumerable<IndexProject>>.Success(response, string.Empty);
        }
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetListProjectUserHasOwner(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return ServicesResult<IEnumerable<IndexProject>>.Failure("id is request");

            var roles = await _unitOfWork.RoleInProjectRepository.GetAllAsync();
            if (roles.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(roles.Message);
            var roleOwner = roles.Data.Where(x => x.RoleName == "Owner").FirstOrDefault();

            var projectJoined = await _context.MemberProject.Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == roleOwner.Id).ToListAsync();
            if (projectJoined == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("User is not join any projects");
            var user = await _unitOfWork.ApplicationUserRepository.GetValueByPrimaryKey(userId);
            if(user.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(user.Message);
            /// find position of user in each project
            var positionList = new List<PositionInProject>();
            foreach (var index in projectJoined)
            {
                var position = await _unitOfWork.PositionInProjectRepository.GetValueByPrimaryKey(index.PositionInProjectId);
                if (position.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(position.Message);
                positionList.Add(position.Data);
            }
            var projectList = new List<IndexProject>();
            foreach (var index in positionList)
            {
                var project = await _unitOfWork.ProjectRepository.GetValueByPrimaryKey(index.ProjectId);
                if (project.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(project.Message);
                var indexProject = new IndexProject()
                {
                    OwnerName = user.Data.UserName ??"Error",
                    ProjectName = project.Data.ProjectName??"Error",
                    OwnerAvata = user.Data.PathImage ?? "Error",
                    ProjectId = index.ProjectId,
                };
                projectList.Add(indexProject);
            }
            return ServicesResult<IEnumerable<IndexProject>>.Success(projectList, string.Empty);
        }
    }
}
