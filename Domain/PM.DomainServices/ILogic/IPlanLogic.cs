using PM.DomainServices.Models.plans;
using PM.DomainServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.DomainServices.ILogic
{
    public interface IPlanLogic
    {
        Task<ServicesResult<IEnumerable<IndexPlan>>> GetPlansInProject(string projectId);
    }
}
