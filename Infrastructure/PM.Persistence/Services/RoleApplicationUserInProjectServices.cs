using Microsoft.EntityFrameworkCore;
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
    public class RoleApplicationUserInProjectServices : IRoleApplicationUserInProjectServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<RoleApplicationUserInProjectDTO> _repository;
        public RoleApplicationUserInProjectServices(ApplicationDbContext context, IRepository<RoleApplicationUserInProjectDTO> repository)
        {
            _context = context;
            _repository = repository;
        }
        public async Task<bool> AddAsync(RoleApplicationUserInProjectDTO roleApplicationUserInProjectDTO)
        {
            return await _repository.AddAsync(roleApplicationUserInProjectDTO);
        }
        public async Task<bool> UpdateAsync(string Id, RoleApplicationUserInProjectDTO roleApplicationUserInProjectDTO)
        {
            return await _repository.UpdateAsync(Id, roleApplicationUserInProjectDTO);
        }
        public async Task<bool> RemoveAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }
        public async Task<IEnumerable<RoleApplicationUserInProjectDTO>> GetProjectsUserJoined(string userid)
        {
            try
            {
                var getData = await _context.RoleApplicationUserInProject.Where(x => x.ApplicationUserId == userid).ToArrayAsync();
                return getData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<RoleApplicationUserInProjectDTO>();
            }
        }
        public async Task<IEnumerable<RoleApplicationUserInProjectDTO>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<RoleApplicationUserInProjectDTO> GetRoleApplicationUserInProjectById(string Id)
        {
            return await _repository.GetValueAsync(Id);
        }

    }
}
