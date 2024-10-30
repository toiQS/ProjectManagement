using PM.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IPositionWorkOfMemberServices
    {
        public Task<bool> AddAsync(PositionWorkOfMemberDTO positionWorkOfMemberDTO);
        public Task<bool> RemoveAsync(string id);
        public Task<bool> UpdateAsync(string id, PositionWorkOfMemberDTO positionWorkOfMemberDTO);
        public Task<IEnumerable<PositionWorkOfMemberDTO>> GetAllAsync();
        public Task<PositionWorkOfMemberDTO> GetPositionWorkOfMemberById(string id);
        public Task<PositionWorkOfMemberDTO> GetPositionWorkOfMemberByRoleApplicationUserIdAndPositionInProjectId(string userId, string positionWorkOfMemberId);
    }
}
