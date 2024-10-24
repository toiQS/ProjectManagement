using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Repository.repository
{
    public interface IRepository<T> where T : class
    {
        public Task<bool> Add(T entity);
        public Task<bool> Update(string id, T entity);
        public Task<bool> Delete(string id);
    }
}
