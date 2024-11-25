using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using PM.Domain;
using PM.DomainServices.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.DomainServices.ILogic
{
    public interface IProjectLogic
    {
        public Task<ServicesResult<IEnumerable<Project>>> GetProjectsUserHasJoined(string userId);
        public Task<ServicesResult<bool>> AddProject(string userId, string projectName, string projectDescription, string projectVersion, string projectStatus);
        public Task<ServicesResult<bool>> UpdateIsDelete(string userId, string projectId);
        public Task<ServicesResult<bool>> UpdateIsAccess(string userId, string projectId);
        public Task<ServicesResult<bool>> DeleteProject(string userId, string projectId);
    }
}
