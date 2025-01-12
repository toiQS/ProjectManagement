using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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
        //intialize primary value
        private List<Project> _projects;
        private List<Status> _statuses;

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
        //private async Task<ServicesResult<IndexProject>> GetIndexProject(string projectId)
        //{
           
        //    var project = new 
        //}
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
          

            var getInfo = _statuses.Data.Where(x => x.Id == statusId).FirstOrDefault();
            if(getInfo == null) return ServicesResult<string>.Failure($"can't get any this status {statusId}");
            return ServicesResult<string>.Success(getInfo.Value, string.Empty);
        }

        #endregion

        #region support method
        #endregion
        #region primary method
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetIndexProjects()
        {
            var result = new List<IndexProject>();
            if( _projects == null ) return ServicesResult<IEnumerable<IndexProject>>.Success(new List<IndexProject>(), "No data of project in databse");
            foreach (var project in _projects)
            {

                //var ownerInfo = await _mem
                var index = new IndexProject()
                {
                    
                };
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

        }
        #endregion
    }
}
