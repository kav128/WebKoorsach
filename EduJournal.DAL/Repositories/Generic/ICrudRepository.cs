using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EduJournal.DAL.Entities;
using EduJournal.DAL.Exceptions;

namespace EduJournal.DAL.Repositories.Generic
{
    /// <summary>
    /// Represents a generic interface for CRUD repository.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public interface ICrudRepository<T> where T : DbEntity
    {
        /// <summary>
        /// Gets an entity with specified id.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous get operation. The task result contains an entity.</returns>
        /// <exception cref="IncorrectIdentifierException">Entity id is negative.</exception>
        /// <exception cref="DataException">An error is occurred while trying to access the database.</exception>
        public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous get operation. The task result contains a collection of entities.</returns>
        /// <exception cref="DataException">An error is occurred while trying to access the database.</exception>
        public Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets filtered collection of entities.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="predicates">A collection of <see cref="Expression"/> which describes data filters.</param>
        /// <returns>A task that represents the asynchronous get operation. The task result contains a collection of entities.</returns>
        /// <exception cref="DataException">An error is occurred while trying to access the database.</exception>
        public Task<IEnumerable<T>> GetFilteredAsync(CancellationToken cancellationToken = default, params Expression<Func<T, bool>>[] predicates);

        /// <summary>
        /// Adds a new entity or updates it if entity exists.
        /// </summary>
        /// <param name="entity">Entity instance.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous get operation. The task result contains id of entity.</returns>
        /// <exception cref="IncorrectIdentifierException">Entity id is negative.</exception>
        /// <exception cref="DataException">An error is occurred while trying to access the database.</exception>
        /// <exception cref="EntityNotFoundException">The entity with specified id does not exist.</exception>
        public Task<int> SaveAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes entity.
        /// </summary>
        /// <param name="entity">Entity instance.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        /// <exception cref="IncorrectIdentifierException">Entity id is negative.</exception>
        /// <exception cref="DataException">An error is occurred while trying to access the database.</exception>
        /// <exception cref="EntityNotFoundException">The entity with specified id does not exist.</exception>
        public Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    }
}
