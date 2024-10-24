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
    public class PositionWorkInProjectServices : IPositionWorkInProjectServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<PositionWorkOfMemberDTO> _repository;
        public PositionWorkInProjectServices(ApplicationDbContext context, IRepository<PositionWorkOfMemberDTO> repository)
        {
            _repository = repository;
            _context = context;
        }
        public async Task<bool> AddAsync(PositionWorkOfMemberDTO positionWorkOfMemberDTO)
        {
            return await _repository.AddAsync(positionWorkOfMemberDTO);
        }
        public async Task<bool> RemoveAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }
        public async Task<bool> UpdateAsync(string id, PositionWorkOfMemberDTO positionWorkOfMemberDTO)
        {
            return await _repository.UpdateAsync(id, positionWorkOfMemberDTO);
        }
    }
}
