using Microsoft.EntityFrameworkCore;
using PM.Domain.DTOs;
using PM.Persistence.Context;
using PM.Persistence.IServices;

namespace PM.Persistence.Services
{
    public class ProjectServices : IProjectServices
    {
        private readonly ApplicationDbContext _context;
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
        public async Task<bool> IsExistProjectName(string Name)
        {
            try
            {
                var isCheck = await _context.ProjectDTO.AnyAsync(x => x.ProjectName == Name);
                return isCheck;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return true;
            }
        }
        public async Task<ProjectDTO> GetProjectAsync(string projectId)
        {
            try
            {
                var getProject = await _context.ProjectDTO.Where(x => x.Id == projectId).FirstOrDefaultAsync();
                if (getProject == null) return new ProjectDTO();
                return getProject;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return new ProjectDTO();    
            }
        }
    }
}
