using PM.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IPositionWorkInProjectServices
    {
        public Task<bool> AddAsync(PositionWorkOfMemberDTO positionWorkOfMemberDTO);
        public Task<bool> RemoveAsync(string id);
        public Task<bool> UpdateAsync(string id, PositionWorkOfMemberDTO positionWorkOfMemberDTO);
    }
}
