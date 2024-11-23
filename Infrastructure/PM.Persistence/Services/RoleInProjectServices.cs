using Microsoft.AspNetCore.Hosting;
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
    public class RoleInProjectServices : IRoleInProjectServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<RoleInProject> _repository;
        public RoleInProjectServices(ApplicationDbContext context, IRepository<RoleInProject> repository)
        {
            _context = context;
            _repository = repository;
        }
        public async Task<bool> AddAsync(RoleInProject roleInProject)
        {
            return await _repository.AddAsync(roleInProject);
        }
        public async Task<bool> RemoveAsync(string Id)
        {
            return await _repository.DeleteAsync(Id);
        }
        public async Task<bool> UpdateAsync(string id, RoleInProject roleInProjectDTO)
        {
            return await _repository.UpdateAsync(id, roleInProjectDTO);
        }
        public async Task<string> GetNameRoleByRoleId(string roleId)
        {
            try
            {
                var getRole =  await _repository.GetValueAsync(roleId);
                if (getRole == null) return string.Empty;
                return getRole.RoleName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
        public async Task<IEnumerable<RoleInProject>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<RoleInProject> GetRoleInProjectByRoleId(string Id)
        {
            return await _repository.GetValueAsync(Id);
        }
    }
}
