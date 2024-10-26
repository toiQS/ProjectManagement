using PM.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IPlanServices
    {
        public Task<bool> AddAsync(PlanDTO planDTO);
        public Task<bool> RemoveAsync(string Id);
        public Task<bool> Update(string Id, PlanDTO planDTO);
    }
}
