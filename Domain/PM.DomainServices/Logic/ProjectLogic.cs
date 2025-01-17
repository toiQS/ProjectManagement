using PM.Domain;
using PM.DomainServices.ILogic;
using PM.DomainServices.Models;
using PM.DomainServices.Models.projects;
using PM.Persistence.IServices;

namespace PM.DomainServices.Logic
{
    public class ProjectLogic : IProjectLogic
    {
        //intialize services
        private readonly IProjectServices _projectServices;
        private readonly IStatusServices _statusServices;
        //intialize logic
        private readonly IUserLogic _userLogic;
        private readonly IMemberLogic _memberLogic;
        private readonly IPlanLogic _planLogic;
        //intialize primary value
        private List<Project> _projects;
        private List<Status> _statuses;

        public ProjectLogic(IProjectServices projectServices, IStatusServices statusServices, IUserLogic userLogic, IMemberLogic memberLogic, IPlanLogic planLogic)
        {
            _projectServices = projectServices;
            _statusServices = statusServices;
            _userLogic = userLogic;
            _memberLogic = memberLogic;
            _planLogic = planLogic;
            IntializeProject();
        }


        #region private method
        private async Task<ServicesResult<IEnumerable<Project>>> GetProjectAsync()
        {
            var projects = await _projectServices.GetAllAsync();
            if (projects.Data == null) return ServicesResult<IEnumerable<Project>>.Success(new List<Project>(), "No data of project in databse");
            if(projects.Status== false) return ServicesResult<IEnumerable<Project>>.Failure(projects.Message);
            return ServicesResult<IEnumerable<Project>>.Success(projects.Data, string.Empty);
        }
        private void IntializeProject()
        {
            var projects = new ServicesResult<IEnumerable<Project>>();
            do
            {
                projects = GetProjectAsync().GetAwaiter().GetResult();
            }
            while (projects.Status == false);
            _projects = projects.Data.ToList();
        }
        
        private async Task<ServicesResult<string>> GetAllStatus()
        {
            var statuses = await _statusServices.GetAllAsync();
            if (statuses.Data == null) return ServicesResult<string>.Success(string.Empty, "no status in database");
            if (statuses.Status == false) return ServicesResult<string>.Failure(statuses.Message);
            _statuses = statuses.Data.ToList();
            return ServicesResult<string>.Success("Success", string.Empty);
        }
        private async Task<ServicesResult<string>> GetStatusInfo(int statusId)
        {
            if (statusId == 0) return ServicesResult<string>.Failure("");
          

            var getInfo = _statuses.Where(x => x.Id == statusId).FirstOrDefault();
            if(getInfo == null) return ServicesResult<string>.Failure($"can't get any this status {statusId}");
            return ServicesResult<string>.Success(getInfo.Value, string.Empty);
        }
       
        #endregion

        #region support method
        public async Task<ServicesResult<IndexProject>> GetIndexProject(string projectId)
        {

            if (string.IsNullOrEmpty(projectId)) return ServicesResult<IndexProject>.Failure("");
            
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (project.Data == null || project.Data == null) return ServicesResult<IndexProject>.Failure(project.Message);
            if (project.Data.IsDeleted == true) ServicesResult<IndexProject>.Success(new IndexProject(), string.Empty);
            var owner = await _memberLogic.GetInfoOfOwnerInProjectAsync(projectId);
            if (owner.Status == false) return ServicesResult<IndexProject>.Failure(owner.Message);
            var result = new IndexProject()
            {
                OwnerName = owner.Data.UserName,
                OwnerAvata = owner.Data.UserAvata,
                ProjectName = project.Data.ProjectName,
                ProjectId = projectId,
            };
            return ServicesResult<IndexProject>.Success(result, string.Empty);
        }
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserHasJoined(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return ServicesResult<IEnumerable<IndexProject>>.Failure("");
            var projects = await _memberLogic.GetProjectsUserHasJoinedByUserIdAsync(userId);
            if (projects.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Success(null, projects.Message);
            if(projects.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(projects.Message);
            var result = new List<IndexProject>();
            foreach (var item in projects.Data)
            {
                var itemProject = await GetIndexProject(item);
                if(itemProject.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(itemProject.Message);
                if (itemProject.Data == null) continue;
                result.Add(itemProject.Data);
            }
            return ServicesResult<IEnumerable<IndexProject>>.Success(result, string.Empty);
        }
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserHasOwn(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return ServicesResult<IEnumerable<IndexProject>>.Failure("");
            var projects = await _memberLogic.GetProjectsUserOwnsAsync(userId);
            if (projects.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Success(null, projects.Message);
            if (projects.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(projects.Message);
            var result = new List<IndexProject>();
            foreach (var item in projects.Data)
            {
                var itemProject = await GetIndexProject(item);
                if (itemProject.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(itemProject.Message);
                if (itemProject.Data == null) continue;
                result.Add(itemProject.Data);
            }
            return ServicesResult<IEnumerable<IndexProject>>.Success(result, string.Empty);
        }

        #endregion
        #region primary method
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetIndexProjects()
        {
            var result = new List<IndexProject>();
            if( _projects == null ) return ServicesResult<IEnumerable<IndexProject>>.Success(new List<IndexProject>(), "No data of project in databse");
            foreach (var project in _projects)
            {
                var index = await GetIndexProject(project.Id);
                if (index.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(index.Message);
                if (index.Data == null)continue;
                result.Add(index.Data);
            }
            return ServicesResult<IEnumerable<IndexProject>>.Success(result, string.Empty);
        }
        public async Task<ServicesResult<DetailProject>> GetDetailProject(string projectId)
        {
            if (string.IsNullOrEmpty(projectId)) return ServicesResult<DetailProject>.Failure("");

            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if(project.Data == null || project.Data == null) return ServicesResult<DetailProject>.Failure(project.Message);

            var detail = new DetailProject()
            {
                ProjectId = projectId,
                ProjectDescription = project.Data.ProjectDescription,
                ProjectName = project.Data.ProjectName,
                CreateAt = project.Data.CreateAt,
                EndAt = project.Data.EndAt,
                StartAt = project.Data.StartAt,
                IsAccessed = project.Data.IsAccessed,
                IsDeleted = project.Data.IsDeleted,
                IsDone = project.Data.IsDone,
            };

            var getStatus = await GetStatusInfo(project.Data.StatusId);
            if(getStatus.Status ==false ) return ServicesResult<DetailProject>.Failure(getStatus.Message);
            detail.Status = getStatus.Data;

            var user = await _memberLogic.GetInfoOfOwnerInProjectAsync(projectId);
            if(user.Status == false ) return ServicesResult<DetailProject>.Failure(user.Message);
            detail.OwnerName = user.Data.UserName;
            detail.OwnerAvata = user.Data.UserAvata;

            var members = await _memberLogic.GetMembersInProjectAsync(projectId);
            if(members.Status == false ) return ServicesResult<DetailProject>.Failure(members.Message); 
            if(members.Data == null) return ServicesResult<DetailProject>.Success(detail, members.Message);
            detail.Members = members.Data.ToList();
            var plans = await _planLogic.GetPlansInProject(projectId);
            if(plans.Status == false) return ServicesResult<DetailProject>.Failure(plans.Message);
            if (plans.Data == null) return ServicesResult<DetailProject>.Success(detail, plans.Message);
            detail.Plans = plans.Data.ToList();
            return ServicesResult<DetailProject>.Success(detail, string.Empty);

        }
        #endregion
    }
}
