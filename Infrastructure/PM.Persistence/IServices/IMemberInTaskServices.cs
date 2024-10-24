using PM.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IMemberInTaskServices
    {
        public Task<bool> AddAsync(MemberInTaskDTO memberInTaskDTO);
        public Task<bool> RemoveAsync(string id);
        public Task<bool> UpdateAsync(string Id, MemberInTaskDTO memberInTaskDTO);
    }
}
