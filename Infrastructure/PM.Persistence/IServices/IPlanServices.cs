using PM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IPlanServices
    {
        public Task<bool> AddAsync(Plan planDTO);
        public Task<bool> RemoveAsync(string Id);
        public Task<bool> Update(string Id, Plan planDTO);
        public Task<IEnumerable<Plan>> GetAllAsync();
        public Task<Plan> GetById(string Id);
    }
}
