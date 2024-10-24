using PM.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IPlanInProjectServices
    {
        public Task<bool> AddAsync(PlanInProjectDTO planInProjectDTO);
        public Task<bool> RemoveAsync(string id);
        public Task<bool> UpdateAsync(string id, PlanInProjectDTO planInProjectDTO);
    }
}
