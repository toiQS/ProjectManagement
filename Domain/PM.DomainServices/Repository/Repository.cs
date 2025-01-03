using Microsoft.EntityFrameworkCore;
using PM.Persistence.Context;
using Shared;

namespace PM.DomainServices.Repository
{
    /// <summary>
    /// Generic repository implementation for interacting with the database.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        #region Fields

        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves all entities of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A <see cref="ServicesResult{T}"/> containing the list of entities.</returns>
        public async Task<ServicesResult<IEnumerable<T>>> GetAllAsync()
        {
            try
            {
                var getData = await _dbSet.AsNoTracking().ToArrayAsync();
                return ServicesResult<IEnumerable<T>>.Success(getData);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<T>>.Failure($"Database error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves an entity by its primary key.
        /// </summary>
        /// <param name="id">The primary key of the entity.</param>
        /// <returns>A <see cref="ServicesResult{T}"/> containing the entity.</returns>
        public async Task<ServicesResult<T>> GetValueByPrimaryKeyAsync(string id)
        {
            try
            {
                var getData = await _dbSet.FindAsync(id);
                return getData != null ? ServicesResult<T>.Success(getData) : ServicesResult<T>.Failure("Entity not found");
            }
            catch (Exception ex)
            {
                return ServicesResult<T>.Failure($"Database error: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A <see cref="ServicesResult{bool}"/> indicating the operation result.</returns>
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
                return ServicesResult<bool>.Failure($"Database error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>A <see cref="ServicesResult{bool}"/> indicating the operation result.</returns>
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
                return ServicesResult<bool>.Failure($"Database error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes an entity by its primary key.
        /// </summary>
        /// <param name="id">The primary key of the entity.</param>
        /// <returns>A <see cref="ServicesResult{bool}"/> indicating the operation result.</returns>
        public async Task<ServicesResult<bool>> DeleteAsync(string id)
        {
            try
            {
                var getEntity = await _dbSet.FindAsync(id);
                if (getEntity == null)
                {
                    return ServicesResult<bool>.Failure("Entity not found");
                }
                _dbSet.Remove(getEntity);
                await _context.SaveChangesAsync();
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Failure($"Database error: {ex.Message}");
            }
        }

        #endregion
    }
}
