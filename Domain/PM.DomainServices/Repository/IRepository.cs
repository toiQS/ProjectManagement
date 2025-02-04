using PM.DomainServices.Models;

namespace PM.DomainServices.Repository
{
    public interface IRepository<T> where T : class
    {
        public Task<ServicesResult<IEnumerable<T>>> GetAllAsync();
        public Task<ServicesResult<T>> GetValueByPrimaryKey(string primaryKey);
        public Task<ServicesResult<T>> GetValueByPrimaryKey(int primaryKey);
        public Task<ServicesResult<bool>> AddAsync(T entity);
        public Task<ServicesResult<bool>> UpdateAsync(T entity);
        public Task<ServicesResult<bool>> DeleteAsync(string primaryKey);
        public Task<ServicesResult<bool>> DeleteAsync(int primaryKey);
    }
}
