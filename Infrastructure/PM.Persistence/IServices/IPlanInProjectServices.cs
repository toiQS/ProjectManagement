using PM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IPlanInProjectServices
    {
        public Task<bool> AddAsync(PlanInProject planInProjectDTO);
        public Task<bool> RemoveAsync(string id);
        public Task<bool> UpdateAsync(string id, PlanInProject planInProjectDTO);
        public Task<PlanInProject> GetByIdAsync(string id);
        public Task<IEnumerable<PlanInProject>> GetAllAsync();
        public Task<IEnumerable<PlanInProject>> GetPlanInProjectsByProjectId(string projectId);
    }
}
