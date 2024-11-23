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
    public class PlanServices : IPlanServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Plan> _planRepository;
        public PlanServices(ApplicationDbContext context, IRepository<Plan> planRepository)
        {
            _context = context;
            _planRepository = planRepository;
        }
        public async Task<bool> AddAsync(Plan planDTO)
        {
            return await _planRepository.AddAsync(planDTO);
        }
        public async Task<bool> RemoveAsync(string Id)
        {
            return await _planRepository.DeleteAsync(Id);
        }
        public async Task<bool> Update(string Id, Plan planDTO)
        {
            return await _planRepository.UpdateAsync(Id, planDTO);
        }
        public async Task<IEnumerable<Plan>> GetAllAsync()
        {
            return await _planRepository.GetAllAsync();
        }
        public async Task<Plan> GetById(string Id)
        {
            return await _planRepository.GetValueAsync(Id);
        }
    }
}
