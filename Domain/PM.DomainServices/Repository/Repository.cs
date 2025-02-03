using Microsoft.EntityFrameworkCore;
using PM.DomainServices.Models;
using PM.Persistence.Context;
using System.Linq.Expressions;

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

        #region Basic CRUD Operations

        // Lấy tất cả dữ liệu
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

        // Lấy dữ liệu theo Primary Key (string)
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

        // Lấy dữ liệu theo Primary Key (int)
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

        // Thêm mới dữ liệu
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

        // Cập nhật dữ liệu
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

        // Xóa dữ liệu theo Primary Key (string)
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

        // Xóa dữ liệu theo Primary Key (int)
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

        #endregion

        #region Advanced Queries

        // Lấy dữ liệu phân trang
        public async Task<ServicesResult<IEnumerable<T>>> GetPagedDataAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return ServicesResult<IEnumerable<T>>.Failure("Invalid pagination parameters");

            try
            {
                var data = await _dbSet.AsNoTracking()
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();

                return ServicesResult<IEnumerable<T>>.Success(data, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<T>>.Failure($"Database access error: {ex.Message}");
            }
        }

        // Lọc dữ liệu theo điều kiện
        public async Task<ServicesResult<IEnumerable<T>>> GetWithFilterAsync(Expression<Func<T, bool>> filter)
        {
            if (filter == null)
                return ServicesResult<IEnumerable<T>>.Failure("Filter is required");

            try
            {
                var data = await _dbSet.AsNoTracking().Where(filter).ToListAsync();
                return ServicesResult<IEnumerable<T>>.Success(data, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<T>>.Failure($"Database access error: {ex.Message}");
            }
        }

        // Lấy số lượng bản ghi
        public async Task<ServicesResult<int>> GetCountAsync()
        {
            try
            {
                var count = await _dbSet.CountAsync();
                return ServicesResult<int>.Success(count, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<int>.Failure($"Database access error: {ex.Message}");
            }
        }

        // Lấy bản ghi đầu tiên thỏa mãn điều kiện
        public async Task<ServicesResult<T>> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter)
        {
            if (filter == null)
                return ServicesResult<T>.Failure("Filter is required");

            try
            {
                var entity = await _dbSet.AsNoTracking().FirstOrDefaultAsync(filter);
                return entity != null
                    ? ServicesResult<T>.Success(entity, string.Empty)
                    : ServicesResult<T>.Failure("No data found");
            }
            catch (Exception ex)
            {
                return ServicesResult<T>.Failure($"Database access error: {ex.Message}");
            }
        }

        // Lấy dữ liệu đã sắp xếp
        public async Task<ServicesResult<IEnumerable<T>>> GetWithSortingAsync(Expression<Func<T, object>> sortBy, bool descending = false)
        {
            try
            {
                var query = descending ? _dbSet.AsNoTracking().OrderByDescending(sortBy) : _dbSet.AsNoTracking().OrderBy(sortBy);
                var result = await query.ToListAsync();
                return ServicesResult<IEnumerable<T>>.Success(result, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<T>>.Failure($"Database access error: {ex.Message}");
            }
        }

        // Thực thi truy vấn SQL thô
        public async Task<ServicesResult<IEnumerable<TResult>>> ExecuteRawSqlAsync<TResult>(string sql, params object[] parameters)
        {
            try
            {
                var result = await _dbSet.FromSqlRaw(sql, parameters).ToListAsync();
                return ServicesResult<IEnumerable<TResult>>.Success((IEnumerable<TResult>)result, string.Empty);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<TResult>>.Failure($"Database access error: {ex.Message}");
            }
        }

        #endregion
    }
}
