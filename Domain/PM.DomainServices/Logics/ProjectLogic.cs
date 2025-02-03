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
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetIndexProjectsAsync()
        {
            var response = new List<IndexProject>();
            var projects = await _unitOfWork.ProjectRepository.GetAllAsync();
            if(!projects.Status) return ServicesResult<IEnumerable<IndexProject>>.Failure(projects.Message);
            if (projects.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Success(response, projects.Message);
            foreach (var project in projects.Data)
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
                if (roles.Status == false   ) return ServicesResult<IEnumerable<IndexProject>>.Failure(roles.Message);
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
            return ServicesResult<IEnumerable<IndexProject>>.Success(response,string.Empty);
        }
    }
}
