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
    public class PlanServices : IPlanServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<PlanDTO> _planRepository;
        public PlanServices(ApplicationDbContext context, IRepository<PlanDTO> planRepository)
        {
            _context = context;
            _planRepository = planRepository;
        }
        public async Task<bool> AddAsync(PlanDTO planDTO)
        {
            return await _planRepository.AddAsync(planDTO);
        }
        public async Task<bool> RemoveAsync(string Id)
        {
            return await _planRepository.DeleteAsync(Id);
        }
        public async Task<bool> Update(string Id, PlanDTO planDTO)
        {
            return await _planRepository.UpdateAsync(Id, planDTO);
        }
    }
}
