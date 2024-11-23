using PM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IProjectServices
    {
        public Task<bool> AddAsync(Project projectDTO);
        public Task<bool> RemoveAsync(string Id);
        public Task<bool> UpdateAsync(string Id, Project projectDTO);
        public Task<bool> IsExistProjectName(string Name);
        public Task<Project> GetProjectAsync(string projectId);
        public Task<IEnumerable<Project>> GetProjectsByProjectName(string ProjectName);
    }
}
