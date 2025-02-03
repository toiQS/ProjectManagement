using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public ProjectLogic(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        #region Helper Methods

        private async Task<ServicesResult<ApplicationUser>> GetUserByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ServicesResult<ApplicationUser>.Failure("User ID is required");

            var user = await _unitOfWork.ApplicationUserRepository.GetValueByPrimaryKey(userId);
            if (!user.Status || user.Data == null)
                return ServicesResult<ApplicationUser>.Failure(user.Message);

            return ServicesResult<ApplicationUser>.Success(user.Data, string.Empty);
        }

        private async Task<ServicesResult<PositionInProject>> GetPositionInProjectAsync(string positionId)
        {
            var position = await _unitOfWork.PositionInProjectRepository.GetValueByPrimaryKey(positionId);
            if (!position.Status)
                return ServicesResult<PositionInProject>.Failure(position.Message);

            return ServicesResult<PositionInProject>.Success(position.Data, string.Empty);
        }

        private async Task<ServicesResult<IEnumerable<RoleInProject>>> GetRolesAsync()
        {
            var roles = await _unitOfWork.RoleInProjectRepository.GetAllAsync();
            if (!roles.Status || roles.Data == null)
                return ServicesResult<IEnumerable<RoleInProject>>.Failure("Roles not found");

            return ServicesResult<IEnumerable<RoleInProject>>.Success(roles.Data, string.Empty);
        }

        #endregion

        #region Get Index Projects

        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetIndexProjectsAsync()
        {
            try
            {
                var projectsResult = await _unitOfWork.ProjectRepository.GetAllAsync();
                if (!projectsResult.Status || projectsResult.Data == null)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure("No projects found");

                var projects = projectsResult.Data;
                var projectIds = projects.Select(p => p.Id).ToList();

                var positions = await _context.PositionInProject
                    .Where(x => projectIds.Contains(x.ProjectId))
                    .ToListAsync();

                var positionIds = positions.Select(p => p.Id).ToList();
                var members = await _context.MemberProject
                    .Where(x => positionIds.Contains(x.PositionInProjectId))
                    .ToListAsync();

                var rolesResult = await GetRolesAsync();
                if (!rolesResult.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(rolesResult.Message);

                var roleOwner = rolesResult.Data.FirstOrDefault(x => x.RoleName == "Owner");
                if (roleOwner == null)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure("Owner role not found");

                var owners = members.Where(m => m.RoleInProjectId == roleOwner.Id).ToList();
                var userDictionary = new Dictionary<string, ApplicationUser>();

                foreach (var owner in owners)
                {
                    var userResult = await GetUserByIdAsync(owner.ApplicationUserId);
                    if (userResult.Status && userResult.Data != null)
                    {
                        userDictionary[owner.ApplicationUserId] = userResult.Data;
                    }
                }

                var response = projects.Select(project =>
                {
                    var owner = owners.FirstOrDefault(o => positions.Any(p => p.ProjectId == project.Id && p.Id == o.PositionInProjectId));
                    if (owner == null || !userDictionary.ContainsKey(owner.ApplicationUserId)) return null;

                    var user = userDictionary[owner.ApplicationUserId];
                    return new IndexProject()
                    {
                        OwnerAvata = user.PathImage ?? "Error",
                        OwnerName = user.UserName ?? "Error",
                        ProjectName = project.ProjectName,
                        ProjectId = project.Id,
                    };
                }).Where(p => p != null).ToList();

                return ServicesResult<IEnumerable<IndexProject>>.Success(response, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure($"Error: {ex.Message}");
            }
        }

        #endregion

        #region Get Project List User Has Joined

        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectListUserHasJoined(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return ServicesResult<IEnumerable<IndexProject>>.Failure("User ID is required");

                var projectJoined = await _context.MemberProject
                    .Where(x => x.ApplicationUserId == userId)
                    .ToListAsync();

                if (!projectJoined.Any())
                    return ServicesResult<IEnumerable<IndexProject>>.Failure("User has not joined any projects");

                var rolesResult = await GetRolesAsync();
                if (!rolesResult.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(rolesResult.Message);

                var roleOwner = rolesResult.Data.FirstOrDefault(x => x.RoleName == "Owner");
                if (roleOwner == null)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure("Owner role not found");

                var indexProjects = new List<IndexProject>();
                foreach (var project in projectJoined)
                {
                    var positionResult = await GetPositionInProjectAsync(project.PositionInProjectId);
                    if (!positionResult.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(positionResult.Message);

                    var projectResult = await _unitOfWork.ProjectRepository.GetValueByPrimaryKey(positionResult.Data.ProjectId);
                    if (!projectResult.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(projectResult.Message);

                    var userResult = await GetUserByIdAsync(project.ApplicationUserId);
                    if (!userResult.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(userResult.Message);

                    indexProjects.Add(new IndexProject()
                    {
                        OwnerAvata = userResult.Data.PathImage ?? "Error",
                        OwnerName = userResult.Data.UserName ?? "Error",
                        ProjectName = projectResult.Data.ProjectName,
                        ProjectId = projectResult.Data.Id,
                    });
                }

                return ServicesResult<IEnumerable<IndexProject>>.Success(indexProjects, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure($"Error: {ex.Message}");
            }
        }

        #endregion

        #region Get List of Projects User Owns

        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetListProjectUserHasOwner(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<IEnumerable<IndexProject>>.Failure("User ID is required");

            var rolesResult = await GetRolesAsync();
            if (!rolesResult.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(rolesResult.Message);

            var roleOwner = rolesResult.Data.FirstOrDefault(x => x.RoleName == "Owner");
            if (roleOwner == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("Owner role not found");

            var projectJoined = await _context.MemberProject
                .Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == roleOwner.Id)
                .ToListAsync();

            if (!projectJoined.Any())
                return ServicesResult<IEnumerable<IndexProject>>.Failure("User has not joined any projects as an owner");

            var userResult = await GetUserByIdAsync(userId);
            if (!userResult.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(userResult.Message);

            var indexProjects = new List<IndexProject>();
            foreach (var project in projectJoined)
            {
                var positionResult = await GetPositionInProjectAsync(project.PositionInProjectId);
                if (!positionResult.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(positionResult.Message);

                var projectResult = await _unitOfWork.ProjectRepository.GetValueByPrimaryKey(positionResult.Data.ProjectId);
                if (!projectResult.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(projectResult.Message);

                indexProjects.Add(new IndexProject()
                {
                    OwnerName = userResult.Data.UserName ?? "Error",
                    ProjectName = projectResult.Data.ProjectName ?? "Error",
                    OwnerAvata = userResult.Data.PathImage ?? "Error",
                    ProjectId = positionResult.Data.ProjectId,
                });
            }

            return ServicesResult<IEnumerable<IndexProject>>.Success(indexProjects, string.Empty);
        }

        #endregion
    }
}
