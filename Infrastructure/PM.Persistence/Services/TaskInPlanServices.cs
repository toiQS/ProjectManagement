using Microsoft.EntityFrameworkCore;
using PM.Domain;
using PM.Persistence.Context;
using PM.Persistence.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.Services
{
    public class TaskInPlanServices : ITaskInPlanServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<TaskInPlan> _repository;
        public TaskInPlanServices(ApplicationDbContext context, IRepository<TaskInPlan> repository)
        {
            _context = context;
            _repository = repository;
        }
        public async Task<bool> AddAsync(TaskInPlan task)
        {
            return await _repository.AddAsync(task);
        }
        public async Task<bool> UpdateAsync(string id, TaskInPlan taskInPlanDTO)
        {
            return await _repository.UpdateAsync(id, taskInPlanDTO);
        }
        public async Task<bool> RemoveAsync(string Id)
        {
            return await _repository.DeleteAsync(Id);
        }
        public async Task<IEnumerable<TaskInPlan>> GetAllTasks()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<TaskInPlan> GetTaskInPlanById(string Id)
        {
            return await _repository.GetValueAsync(Id);
        }
        public async Task<IEnumerable<TaskInPlan>> GetAllTaskInPlanByPlanId(string planId)
        {
            try
            {
                return await _context.TaskInPlan.Where(x => x.PlanId == planId).ToArrayAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<TaskInPlan>();
            }
        }
    }
}
