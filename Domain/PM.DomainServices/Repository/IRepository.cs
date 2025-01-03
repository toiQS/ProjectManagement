using PM.DomainServices.Models;

namespace PM.DomainServices.Repository
{
    /// <summary>
    /// Generic repository interface for CRUD operations.
    /// </summary>
    /// <typeparam name="T">The entity type to be managed by the repository.</typeparam>
    public interface IRepository<T> where T : class
    {
        #region Read Operations

        /// <summary>
        /// Retrieves all entities from the repository.
        /// </summary>
        /// <returns>An enumerable collection of all entities.</returns>
        Task<ServicesResult<IEnumerable<T>> >GetAllAsync();

        /// <summary>
        /// Retrieves a single entity by its primary key.
        /// </summary>
        /// <param name="id">The primary key of the entity.</param>
        /// <returns>The entity that matches the primary key, or null if not found.</returns>
        Task<ServicesResult<T>> GetValueByPrimaryKeyAsync(string id);

        #endregion

        #region Create Operation

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        Task<ServicesResult<bool>> AddAsync(T entity);

        #endregion

        #region Update Operation

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The updated entity object.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        Task<ServicesResult<bool>> UpdateAsync(T entity);

        #endregion

        #region Delete Operation

        /// <summary>
        /// Deletes an entity from the repository.
        /// </summary>
        /// <param name="id">The primary key of the entity to be deleted.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        Task<ServicesResult<bool>> DeleteAsync(string id);

        #endregion
    }
}
