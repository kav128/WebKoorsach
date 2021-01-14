using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EduJournal.DAL.Entities;
using EduJournal.DAL.Exceptions;
using EduJournal.DAL.Repositories.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduJournal.DAL.Repositories
{
    /// <summary>
    /// Represents a repository of lecturers.
    /// </summary>
    public class LecturerRepository : CrudRepository<Lecturer>
    {
        private readonly ILogger<LecturerRepository> _logger;

        /// <summary>
        /// Initializes a new instance of repository of lecturers.
        /// </summary>
        /// <param name="contextFactory">A factory for creating database context.</param>
        /// <param name="logger">An instance of logger.</param>
        public LecturerRepository(IDbContextFactory<ApplicationContext> contextFactory, ILogger<LecturerRepository> logger) : base(contextFactory)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets a lecturer with specified id.
        /// </summary>
        /// <inheritdoc />
        public override async Task<Lecturer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0) throw new IncorrectIdentifierException("Entity id must be positive int.");
            await using ApplicationContext context = GetContext();

            try
            {
                return await context.Lecturers.Where(lecturer => lecturer.Id == id)
                    .Include(lecturer => lecturer.Courses)
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to retrieve data from database");
                throw new DataException("An error occurred while getting data from database", e);
            }
        }

        /// <summary>
        /// Gets all lecturers.
        /// </summary>
        /// <inheritdoc />
        public override async Task<IEnumerable<Lecturer>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            await using ApplicationContext context = GetContext();

            try
            {
                return await context.Lecturers
                    .Include(lecturer => lecturer.Courses)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to retrieve data from database");
                throw new DataException("An error occurred while getting data from database", e);
            }
        }

        /// <summary>
        /// Gets filtered collection of lecturers.
        /// </summary>
        /// <inheritdoc />
        public override async Task<IEnumerable<Lecturer>> GetFilteredAsync(CancellationToken cancellationToken = default, params Expression<Func<Lecturer, bool>>[] predicates)
        {
            await using ApplicationContext context = GetContext();

            try
            {
                return await predicates.Aggregate<Expression<Func<Lecturer, bool>>, IQueryable<Lecturer>>(context.Lecturers, 
                        (current, predicate) => current.Where(predicate))
                    .Include(lecturer => lecturer.Courses)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to retrieve data from database");
                throw new DataException("An error occurred while getting data from database", e);
            }
        }
        
        /// <summary>
        /// Adds a new lecturer or updates it if lecturer exists.
        /// </summary>
        /// <inheritdoc />
        public override async Task<int> SaveAsync(Lecturer entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id < 0) throw new IncorrectIdentifierException("Entity id must be non-negative int.");
            await using ApplicationContext context = GetContext();

            if (entity.Id == default)
                await context.Lecturers.AddAsync(entity, cancellationToken);
            else
                context.Lecturers.Update(entity);

            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException e)
            {
                _logger.LogError(e, "Unable to update entity which does not exist.");
                throw new EntityNotFoundException("Unable to update entity which does not exist.", e);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to update data in database");
                throw new DataException("An error occurred while updating data in database", e);
            }

            return entity.Id;
        }

        /// <summary>
        /// Deletes a lecturer.
        /// </summary>
        /// <inheritdoc />
        public override async Task DeleteAsync(Lecturer entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id <= 0) throw new IncorrectIdentifierException("Entity id must be positive int.");
            await using ApplicationContext context = GetContext();

            context.Lecturers.Remove(entity);
            
            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException e)
            {
                _logger.LogError(e, "Unable to delete entity which does not exist.");
                throw new EntityNotFoundException("Unable to delete entity which does not exist.", e);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to delete data in database");
                throw new DataException("An error occurred while delete data from database", e);
            }
        }
    }
}
