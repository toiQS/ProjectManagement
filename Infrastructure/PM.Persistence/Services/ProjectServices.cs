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
    public class ProjectServices : IProjectServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRepository<ProjectDTO> _repository;
        public ProjectServices(ApplicationDbContext context, IRepository<ProjectDTO> repository)
        {
            _context = context;
            _repository = repository;
        }
        public async Task<bool> AddAsync(ProjectDTO projectDTO)
        {
            return await _repository.AddAsync(projectDTO);
        }
        public async Task<bool> RemoveAsync(string Id)
        {
            return await _repository.DeleteAsync(Id);
        }
        public async Task<bool> UpdateAsync(string Id, ProjectDTO projectDTO)
        {
            return await _repository.UpdateAsync(Id, projectDTO);
        }
    }
}
