using Microsoft.EntityFrameworkCore.Query.Internal;
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
    public class TaskServices : ITaskServices
    {
        private readonly IRepository<TaskDTO> _taskInPlanDTORepository;
        private readonly ApplicationDbContext _context;
        public TaskServices(ApplicationDbContext context, IRepository<TaskDTO> serviceRepository)
        {
            _context = context;
            _taskInPlanDTORepository = serviceRepository;
        }
        public async Task<bool> AddAsync(TaskDTO taskDTO)
        {
            return await _taskInPlanDTORepository.AddAsync(taskDTO);
        }
        public async Task<bool> RemoveAsync(string id)
        {
            return await _taskInPlanDTORepository.DeleteAsync(id);
        }
        public async Task<bool> UpdateAsync(string id, TaskDTO taskDTO)
        {
            return await _taskInPlanDTORepository.UpdateAsync(id, taskDTO);
        }
        public async Task<IEnumerable<TaskDTO>> GetAllAsync()
        {
            return await _taskInPlanDTORepository.GetAllAsync();
        }
        public async Task<TaskDTO> GetTaskById(string Id)
        {
            return await _taskInPlanDTORepository.GetValueAsync(Id);
        }
    }
}
