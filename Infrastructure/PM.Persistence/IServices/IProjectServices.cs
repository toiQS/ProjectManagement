using PM.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IProjectServices
    {
        public Task<bool> AddAsync(ProjectDTO projectDTO);
        public Task<bool> RemoveAsync(string Id);
        public Task<bool> UpdateAsync(string Id, ProjectDTO projectDTO);

    }
}
