using Microsoft.EntityFrameworkCore;
using PM.Persistence.Context;
using Shared;

namespace PM.DomainServices.Repository
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
        public async Task<ServicesResult<IEnumerable<T>>> GetAllAsync()
        {
            try
            {
                var getData =  await _dbSet.AsNoTracking().ToArrayAsync();
                return ServicesResult<IEnumerable<T>>.Success(getData);
            }
            catch (Exception ex)
            {
                ServicesResult<IEnumerable<T>>.Failure("Having some problems communicating with the database.\nHelp: " + ex.HelpLink);
                throw;
            }
        }
        public async Task<ServicesResult<T>>GetValueByPrimaryKeyAsync(string Id)
        {
            try
            {
                var getData = await _dbSet.FindAsync(Id);
                return ServicesResult<T>.Success(getData);
            }
            catch (Exception ex)
            {
                ServicesResult<IEnumerable<T>>.Failure("Having some problems communicating with the database.\nHelp: " + ex.HelpLink);
                throw;
            }
        }
        public async Task<ServicesResult<bool>> AddAsync(T entity)
        {

            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                ServicesResult<IEnumerable<T>>.Failure("Having some problems communicating with the database.\nHelp: " + ex.HelpLink);
                throw;
            }
        }
        public async Task<ServicesResult<bool>> UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                ServicesResult<IEnumerable<T>>.Failure("Having some problems communicating with the database.\nHelp: " + ex.HelpLink);
                throw;
            }
        }
        public async Task<ServicesResult<bool>> DeleteAsync(string id)
        {
            try
            {
                var getEntity = await _dbSet.FindAsync(id);
                if (getEntity == null)
                {
                    return ServicesResult<bool>.Failure("Not Found");
                }
                _dbSet.Remove(getEntity);
                await _context.SaveChangesAsync();
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                ServicesResult<IEnumerable<T>>.Failure("Having some problems communicating with the database.\nHelp: " + ex.HelpLink);
                throw;
            }

        }
    }
}
