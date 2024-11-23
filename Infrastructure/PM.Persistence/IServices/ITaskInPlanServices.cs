using PM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface ITaskInPlanServices
    {
        public Task<bool> AddAsync(TaskInPlan taskInPlanDTO);
        public Task<bool> RemoveAsync(string Id);
        public Task<bool> UpdateAsync(string id, TaskInPlan taskInPlanDTO);
        public Task<TaskInPlan> GetTaskInPlanById(string Id);
        public Task<IEnumerable<TaskInPlan>> GetAllTasks();
        public Task<IEnumerable<TaskInPlan>> GetAllTaskInPlanByPlanId(string planId);
    }
}
