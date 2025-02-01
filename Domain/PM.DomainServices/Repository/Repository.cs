using Microsoft.EntityFrameworkCore;
using PM.DomainServices.Models;
using PM.Persistence.Context;

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
                return ServicesResult<IEnumerable<T>>.Success(response, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<T>>.Failure($"Database access error: {ex.Message}");
            }
        }

        public async Task<ServicesResult<T>> GetValueByPrimaryKey(string primaryKey)
        {
            if (string.IsNullOrEmpty(primaryKey)) return ServicesResult<T>.Failure("Primary key is required");

            try
            {
                var response = await _dbSet.FindAsync(primaryKey);
                return response != null
                    ? ServicesResult<T>.Success(response, string.Empty)
                    : ServicesResult<T>.Failure("No data found");
            }
            catch (Exception ex)
            {
                return ServicesResult<T>.Failure($"Database access error: {ex.Message}");
            }
        }

        public async Task<ServicesResult<T>> GetValueByPrimaryKey(int primaryKey)
        {
            if (primaryKey <= 0) return ServicesResult<T>.Failure("Invalid primary key");

            try
            {
                var response = await _dbSet.FindAsync(primaryKey);
                return response != null
                    ? ServicesResult<T>.Success(response, string.Empty)
                    : ServicesResult<T>.Failure("No data found");
            }
            catch (Exception ex)
            {
                return ServicesResult<T>.Failure($"Database access error: {ex.Message}");
            }
        }

        public async Task<ServicesResult<bool>> AddAsync(T entity)
        {
            if (entity is null) return ServicesResult<bool>.Failure("Entity is required");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _dbSet.AddAsync(entity);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return ServicesResult<bool>.Success(true, string.Empty);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return ServicesResult<bool>.Failure($"Database access error: {ex.Message}");
                }
            }
        }

        public async Task<ServicesResult<bool>> UpdateAsync(T entity)
        {
            if (entity is null) return ServicesResult<bool>.Failure("Entity is required");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _dbSet.Update(entity);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return ServicesResult<bool>.Success(true, string.Empty);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return ServicesResult<bool>.Failure($"Database access error: {ex.Message}");
                }
            }
        }

        public async Task<ServicesResult<bool>> DeleteAsync(string primaryKey)
        {
            if (string.IsNullOrEmpty(primaryKey)) return ServicesResult<bool>.Failure("Primary key is required");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var entity = await _dbSet.FindAsync(primaryKey);
                    if (entity == null) return ServicesResult<bool>.Failure("No data found");

                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return ServicesResult<bool>.Success(true, string.Empty);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return ServicesResult<bool>.Failure($"Database access error: {ex.Message}");
                }
            }
        }

        public async Task<ServicesResult<bool>> DeleteAsync(int primaryKey)
        {
            if (primaryKey <= 0) return ServicesResult<bool>.Failure("Invalid primary key");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var entity = await _dbSet.FindAsync(primaryKey);
                    if (entity == null) return ServicesResult<bool>.Failure("No data found");

                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return ServicesResult<bool>.Success(true, string.Empty);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return ServicesResult<bool>.Failure($"Database access error: {ex.Message}");
                }
            }
        }
    }
}
