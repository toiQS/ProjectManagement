using Microsoft.AspNetCore.Hosting;
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
    public class RoleInProjectServices : IRoleInProjectServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<RoleInProjectDTO> _repository;
        public RoleInProjectServices(ApplicationDbContext context, IRepository<RoleInProjectDTO> repository)
        {
            _context = context;
            _repository = repository;
        }
        public async Task<bool> AddAsync(RoleInProjectDTO roleInProject)
        {
            return await _repository.AddAsync(roleInProject);
        }
        public async Task<bool> RemoveAsync(string Id)
        {
            return await _repository.DeleteAsync(Id);
        }
        public async Task<bool> UpdateAsync(string id, RoleInProjectDTO roleInProjectDTO)
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
        public async Task<IEnumerable<RoleInProjectDTO>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<RoleInProjectDTO> GetRoleInProjectByRoleId(string Id)
        {
            return await _repository.GetValueAsync(Id);
        }
    }
}
