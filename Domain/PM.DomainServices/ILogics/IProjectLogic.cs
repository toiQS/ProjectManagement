using PM.DomainServices.Models.projects;
using PM.DomainServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.DomainServices.ILogics
{
    public interface IProjectLogic
    {
        Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectListUserHasJoined(string userId);
    }
}
