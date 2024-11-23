using Microsoft.EntityFrameworkCore;
using PM.Persistence.Context;
using PM.Persistence.IServices;

namespace PM.Persistence.Services
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _dbSet.AsNoTracking().ToArrayAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<T>();
            }
        }
        public async Task<T> GetValueByPrimaryKeyAsync(string Id)
        {
            try
            {
                var getData = await _dbSet.FindAsync(Id);
                if (getData == null)
                {
                    throw new Exception("Can't find data");
                }
                return getData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<bool> AddAsync(T entity)
        {

            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        public async Task<bool> UpdateAsync(string id, T entity)
        {
            try
            {
                var getEntity = await _dbSet.FindAsync(id);
                if (getEntity == null)
                {
                    throw new Exception("Can't get data");
                }
                getEntity = entity;
                _dbSet.Update(getEntity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var getEntity = await _dbSet.FindAsync(id);
                if (getEntity == null)
                {
                    throw new Exception("Can't get data");
                }
                _dbSet.Remove(getEntity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }
    }
}
