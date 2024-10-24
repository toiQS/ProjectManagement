using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T> GetValueAsync(string id);
        public Task<bool> AddAsync(T entity);
        public Task<bool> UpdateAsync(string id, T entity);
        public Task<bool> DeleteAsync(string id);
    }
}
