using Microsoft.EntityFrameworkCore;
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
    public class RoleApplicationUserInProjectServices : IRoleApplicationUserInProjectServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<RoleApplicationUserInProject> _repository;
        public RoleApplicationUserInProjectServices(ApplicationDbContext context, IRepository<RoleApplicationUserInProject> repository)
        {
            _context = context;
            _repository = repository;
        }
        public async Task<bool> AddAsync(RoleApplicationUserInProject roleApplicationUserInProjectDTO)
        {
            return await _repository.AddAsync(roleApplicationUserInProjectDTO);
        }
        public async Task<bool> UpdateAsync(string Id, RoleApplicationUserInProject roleApplicationUserInProjectDTO)
        {
            return await _repository.UpdateAsync(Id, roleApplicationUserInProjectDTO);
        }
        public async Task<bool> RemoveAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }
        public async Task<IEnumerable<RoleApplicationUserInProject>> GetProjectsUserJoined(string userid)
        {
            try
            {
                var getData = await _context.RoleApplicationUserInProject.Where(x => x.ApplicationUserId == userid).ToArrayAsync();
                return getData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<RoleApplicationUserInProject>();
            }
        }
        public async Task<IEnumerable<RoleApplicationUserInProject>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<RoleApplicationUserInProject> GetRoleApplicationUserInProjectById(string Id)
        {
            return await _repository.GetValueAsync(Id);
        }
        public async Task<IEnumerable<RoleApplicationUserInProject>> GetRoleApplicationUserInProjectsByProjectId(string projectId)
        {
            try
            {
                var getData = await _context.RoleApplicationUserInProject.Where(x => x.ProjectId == projectId).ToArrayAsync();
                return getData;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}", ex);
                return new List<RoleApplicationUserInProject>();
            }
        }
        public async Task<RoleApplicationUserInProject> GetRoleApplicationUserInProjectByUserIdAndProjectId(string UserId, string ProjectId)
        {
            try
            {
                var getData = await _context.RoleApplicationUserInProject.Where(x =>x.ProjectId == ProjectId && x.ApplicationUserId == UserId).FirstOrDefaultAsync();
                return getData;
            }
            catch( Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new RoleApplicationUserInProject();
            }
        }
    }
}
