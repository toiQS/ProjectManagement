using Microsoft.EntityFrameworkCore;
using PM.Domain;
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
        private readonly IRepository<MemberInTask> _repository;
        public MemberInTaskServices(ApplicationDbContext context, IRepository<MemberInTask> repository)
        {
            _context = context;
            _repository = repository;
        }
        public async Task<bool> AddAsync(MemberInTask memberInTaskDTO)
        {
            return await _repository.AddAsync(memberInTaskDTO);
        }
        public async Task<bool> RemoveAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }
        public async Task<bool> UpdateAsync(string Id, MemberInTask memberInTaskDTO)
        {
            return await _repository.UpdateAsync(Id, memberInTaskDTO);    
        }
        public async Task<IEnumerable<MemberInTask>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<MemberInTask> GetMemberInTaskASync(string id)
        {
            return await _repository.GetValueAsync(id);
        }
        public async Task<IEnumerable<MemberInTask>> GetAllMemberInTaskByPositionWorkOfMemberId(string id)
        {
            try
            {
                return await _context.MemberInTask.Where(x => x.PositionWorkOfMemberId == id).ToArrayAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<MemberInTask>();
            }
        }
    }
}
