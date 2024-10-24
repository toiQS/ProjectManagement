using PM.Domain.DTOs;
using PM.Persistence.Context;
using PM.Persistence.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.memberInTask
{
    public class MemberInTaskServices : IMemberInTaskServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<MemberInTaskDTO> _repository;
        public MemberInTaskServices(ApplicationDbContext context, IRepository<MemberInTaskDTO> repository)
        {
            _context = context;
            _repository = repository;
        }
        public async Task<bool> AddAsync(MemberInTaskDTO memberInTaskDTO)
        {
            return await _repository.AddAsync(memberInTaskDTO);
        }
        public async Task<bool> RemoveAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }
        public async Task<bool> UpdateAsync(string Id, MemberInTaskDTO memberInTaskDTO)
        {
            return await _repository.UpdateAsync(Id, memberInTaskDTO);    
        }
    }
}
