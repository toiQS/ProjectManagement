using PM.Domain;
using PM.DomainServices.ILogic;
using PM.DomainServices.Models;
using PM.DomainServices.Models.projects;
using PM.Persistence.IServices;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.DomainServices.Logic
{
    public class ProjectLogic
    {
        //intialize services
        private readonly IProjectServices _projectServices;
        //intialize logic
        private readonly IUserLogic _userLogic;
        //intialize primary value
        private List<Project> _projects;

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

        #endregion

        #region support method
        #endregion
        #region primary method
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetIndexProjects()
        {
            if( _projects == null ) return ServicesResult<IEnumerable<IndexProject>>.Success(new List<IndexProject>(), "No data of project in databse");
            var infoOwner = await _userLogic.GetInfoOtherUserByUserId()
            var result = _projects.Select(x => new IndexProject()
            {

            });
            return ServicesResult<IEnumerable<IndexProject>>.Success(result, string.Empty);
        }
        #endregion
    }
}
