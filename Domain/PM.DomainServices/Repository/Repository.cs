using Microsoft.EntityFrameworkCore;
using PM.DomainServices.Models;
using PM.Persistence.Context;
using System.Diagnostics;

namespace PM.DomainServices.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();

        }
        public async Task<ServicesResult<IEnumerable<T>>> GetAllAsync()
        {
            try
            {
                var response = await _dbSet.AsNoTracking().AsSplitQuery().ToListAsync();
                if (response == null) return ServicesResult<IEnumerable<T>>.Success(null, "No data in table");
                return ServicesResult<IEnumerable<T>>.Success(response, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<T>>.Failure($"There is a issue when connect to data table. " + ex.Source);
            }
        }
        public async Task<ServicesResult<T>> GetValueByPrimaryKey(string primaryKey)
        {
            if (string.IsNullOrEmpty(primaryKey)) return ServicesResult<T>.Failure("Id is request");
            try
            {
                var response = await _dbSet.FindAsync(primaryKey);
                if (response == null) return ServicesResult<T>.Success(null, "No data in table");
                return ServicesResult<T>.Success(response, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<T>.Failure($"There is a issue when connect to data table. " + ex.Source);
            }
        }
        public async Task<ServicesResult<T>> GetValueByPrimaryKey(int primaryKey)
        {
            if (primaryKey <= 0) return ServicesResult<T>.Failure("Id is request");
            try
            {
                var response = await _dbSet.FindAsync(primaryKey);
                if (response == null) return ServicesResult<T>.Success(null, "No data in table");
                return ServicesResult<T>.Success(response, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<T>.Failure($"There is a issue when connect to data table. " + ex.Source);
            }
        }
        public async Task<ServicesResult<bool>> AddAsync(T entity)
        {
            if (entity is null) return ServicesResult<bool>.Failure("Entity is request");
            using (var trasaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _dbSet.AddRangeAsync(entity);
                    await _context.SaveChangesAsync();
                    await trasaction.CommitAsync();
                    return ServicesResult<bool>.Success(true, string.Empty);
                }
                catch (Exception ex)
                {
                    await trasaction.RollbackAsync();
                    return ServicesResult<bool>.Failure($"There is a issue when connect to data table. " + ex.Source);
                }
            }
        }
        public async Task<ServicesResult<bool>> UpdateAsync(T entity)
        {
            if (entity is null) return ServicesResult<bool>.Failure("Entity is request");
            using (var trasaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _dbSet.UpdateRange(entity);
                    await _context.SaveChangesAsync();
                    await trasaction.CommitAsync();
                    return ServicesResult<bool>.Success(true, string.Empty);
                }
                catch (Exception ex)
                {
                    await trasaction.RollbackAsync();
                    return ServicesResult<bool>.Failure($"There is a issue when connect to data table. " + ex.Source);
                }
            }
        }
        public async Task<ServicesResult<bool>> DeleteAsync(string primaryKey)
        {
            if (primaryKey == null) return ServicesResult<bool>.Failure("Primary Key is request");
            using (var trasaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var response = await _dbSet.FindAsync(primaryKey);
                    if (response == null) return ServicesResult<bool>.Failure("No data in table");
                    _dbSet.Remove(response);
                    await _context.SaveChangesAsync();
                    await trasaction.CommitAsync();
                    return ServicesResult<bool>.Success(true, string.Empty);
                }
                catch (Exception ex)
                {
                    await trasaction.RollbackAsync();
                    return ServicesResult<bool>.Failure($"There is a issue when connect to data table. " + ex.Source);
                }
            }

        }
        public async Task<ServicesResult<bool>> DeleteAsync(int primaryKey)
        {
            if (primaryKey <= 0) return ServicesResult<bool>.Failure("Primary Key is request");
            using (var trasaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var response = await _dbSet.FindAsync(primaryKey);
                    if (response == null) return ServicesResult<bool>.Failure("No data in table");
                    _dbSet.Remove(response);
                    await _context.SaveChangesAsync();
                    await trasaction.CommitAsync();
                    return ServicesResult<bool>.Success(true, string.Empty);
                }
                catch (Exception ex)
                {
                    await trasaction.RollbackAsync();
                    return ServicesResult<bool>.Failure($"There is a issue when connect to data table. " + ex.Source);
                }
            }
        }
    }
}
