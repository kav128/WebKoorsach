using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EduJournal.DAL.Entities;
using EduJournal.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EduJournal.DAL.Repositories.Generic
{
    /// <summary>
    /// Represents a generic CRUD repository.
    /// </summary>
    /// <inheritdoc cref="ICrudRepository{T}"/>
    public abstract class CrudRepository<T> : ICrudRepository<T> where T : DbEntity
    {
        private readonly IDbContextFactory<ApplicationContext> _contextFactory;

        /// <summary>
        /// Initializes a new instance of CRUD repository
        /// </summary>
        /// <param name="contextFactory">Factory for creating database context.</param>
        protected CrudRepository(IDbContextFactory<ApplicationContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <inheritdoc />
        public abstract Task<int> SaveAsync(T entity, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);


        /// <inheritdoc />
        public abstract Task<IEnumerable<T>> GetFilteredAsync(CancellationToken cancellationToken = default, params Expression<Func<T, bool>>[] predicates);

        /// <inheritdoc />
        public abstract Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        
        protected ApplicationContext GetContext()
        {
            ApplicationContext? context = _contextFactory.CreateDbContext();
            if (context is null)
                throw new DbContextCreatingException("Unable to create database context");
            return context;
        }
    }
}
