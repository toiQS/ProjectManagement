using PM.Domain.DTOs;
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
        private readonly IRepository<TaskInPlanDTO> _repository;
        public TaskInPlanServices(ApplicationDbContext context, IRepository<TaskInPlanDTO> repository)
        {
            _context = context;
            _repository = repository;
        }
        public async Task<bool> AddAsync(TaskInPlanDTO task)
        {
            return await _repository.AddAsync(task);
        }
        public async Task<bool> UpdateAsync(string id, TaskInPlanDTO taskInPlanDTO)
        {
            return await _repository.UpdateAsync(id, taskInPlanDTO);
        }
        public async Task<bool> RemoveAsync(string Id)
        {
            return await _repository.DeleteAsync(Id);
        }
        public async Task<IEnumerable<TaskInPlanDTO>> GetAllTasks()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<TaskInPlanDTO> GetTaskInPlanById(string Id)
        {
            return await _repository.GetValueAsync(Id);
        }
    }
}
