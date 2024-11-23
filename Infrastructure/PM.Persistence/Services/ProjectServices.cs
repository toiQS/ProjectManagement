using Microsoft.EntityFrameworkCore;
using PM.Domain;
using PM.Persistence.Context;
using PM.Persistence.IServices;

namespace PM.Persistence.Services
{
    public class ProjectServices : IProjectServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Project> _repository;
        public ProjectServices(ApplicationDbContext context, IRepository<Project> repository)
        {
            _context = context;
            _repository = repository;
        }
        public async Task<bool> AddAsync(Project projectDTO)
        {
            return await _repository.AddAsync(projectDTO);
        }
        public async Task<bool> RemoveAsync(string Id)
        {
            return await _repository.DeleteAsync(Id);
        }
        public async Task<bool> UpdateAsync(string Id, Project projectDTO)
        {
            return await _repository.UpdateAsync(Id, projectDTO);
        }
        public async Task<bool> IsExistProjectName(string Name)
        {
            try
            {
                var isCheck = await _context.Project.AnyAsync(x => x.ProjectName == Name);
                return isCheck;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return true;
            }
        }
        public async Task<Project> GetProjectAsync(string projectId)
        {
            return await _repository.GetValueAsync(projectId);
        }
        public async Task<IEnumerable<Project>> GetProjectsByProjectName(string ProjectName)
        {
            try
            {
                var getData = await _context.Project.Where(x=> x.ProjectName.ToLower().Contains(ProjectName.ToLower())).ToListAsync();
                return getData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return Enumerable.Empty<Project>();
            }
        }

        
    }
}
