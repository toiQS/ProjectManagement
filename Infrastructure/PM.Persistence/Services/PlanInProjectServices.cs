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
    public class PlanInProjectServices : IPlanInProjectServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<PlanInProject> _repository;
        public PlanInProjectServices(ApplicationDbContext context, IRepository<PlanInProject> repository)
        {
            _context = context;
            _repository = repository;
        }
        public async Task<bool> AddAsync(PlanInProject planInProjectDTO)
        {
            return await _repository.AddAsync(planInProjectDTO);
        }
        public async Task<bool> UpdateAsync(string Id, PlanInProject planInProjectDTO)
        {
            return await _repository.UpdateAsync(Id, planInProjectDTO);
        }
        public async Task<bool> RemoveAsync(string Id)
        {
            return await _repository.DeleteAsync(Id);
        }
        public async Task<PlanInProject> GetByIdAsync(string id)
        {
            return await _repository.GetValueAsync(id);
        }
        public async Task<IEnumerable<PlanInProject>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<IEnumerable<PlanInProject>> GetPlanInProjectsByProjectId(string projectId)
        {
            try
            {
                return await _context.PlanInProject.Where(x => x.ProjectId == projectId).ToArrayAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<PlanInProject>();
            }
        }
    }
}
