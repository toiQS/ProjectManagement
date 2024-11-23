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
    public class PositionWorkInProjectServices : IPositionWorkOfMemberServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<PositionWorkOfMember> _repository;
        public PositionWorkInProjectServices(ApplicationDbContext context, IRepository<PositionWorkOfMember> repository)
        {
            _repository = repository;
            _context = context;
        }
        public async Task<bool> AddAsync(PositionWorkOfMember positionWorkOfMemberDTO)
        {
            return await _repository.AddAsync(positionWorkOfMemberDTO);
        }
        public async Task<bool> RemoveAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }
        public async Task<bool> UpdateAsync(string id, PositionWorkOfMember positionWorkOfMemberDTO)
        {
            return await _repository.UpdateAsync(id, positionWorkOfMemberDTO);
        }
        public async Task<IEnumerable<PositionWorkOfMember>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<PositionWorkOfMember> GetPositionWorkOfMemberById(string id)
        {
            return await _repository.GetValueAsync(id);
        }
        public async Task<PositionWorkOfMember> GetPositionWorkOfMemberByRoleApplicationUserIdAndPositionInProjectId(string userId, string positionWorkOfMemberId)
        {
            try
            {
                return await _context.PositionWorkOfMember.Where( x=> x.RoleApplicationUserInProjectId == userId && x.PostitionInProjectId == positionWorkOfMemberId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new PositionWorkOfMember();
            }
        }        
    }
}
