using PM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IMemberInTaskServices 
    {
        public Task<bool> AddAsync(MemberInTask memberInTaskDTO);
        public Task<bool> RemoveAsync(string id);
        public Task<bool> UpdateAsync(string Id, MemberInTask memberInTaskDTO);
        public Task<IEnumerable<MemberInTask>> GetAllAsync();
        public Task<MemberInTask> GetMemberInTaskASync(string id);
        public Task<IEnumerable<MemberInTask>> GetAllMemberInTaskByPositionWorkOfMemberId(string id);
    }
}
