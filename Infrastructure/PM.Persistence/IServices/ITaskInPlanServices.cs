using PM.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface ITaskInPlanServices
    {
        public Task<bool> AddAsync(TaskInPlanDTO taskInPlanDTO);
        public Task<bool> RemoveAsync(string Id);
        public Task<bool> UpdateAsync(string id, TaskInPlanDTO taskInPlanDTO);
    }
}
