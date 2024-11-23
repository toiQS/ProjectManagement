using PM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IPositionWorkOfMemberServices
    {
        public Task<bool> AddAsync(PositionWorkOfMember positionWorkOfMemberDTO);
        public Task<bool> RemoveAsync(string id);
        public Task<bool> UpdateAsync(string id, PositionWorkOfMember positionWorkOfMemberDTO);
        public Task<IEnumerable<PositionWorkOfMember>> GetAllAsync();
        public Task<PositionWorkOfMember> GetPositionWorkOfMemberById(string id);
        public Task<PositionWorkOfMember> GetPositionWorkOfMemberByRoleApplicationUserIdAndPositionInProjectId(string userId, string positionWorkOfMemberId);
    }
}
