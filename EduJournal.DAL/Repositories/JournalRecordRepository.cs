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
    /// Represents a repository of journal records.
    /// </summary>
    public class JournalRecordRepository : CrudRepository<JournalRecord>
    {
        private readonly ILogger<JournalRecordRepository> _logger;

        /// <summary>
        /// Initializes a new instance of repository of journal records.
        /// </summary>
        /// <param name="contextFactory">A factory for creating database context.</param>
        /// <param name="logger">An instance of logger.</param>
        public JournalRecordRepository(IDbContextFactory<ApplicationContext> contextFactory, ILogger<JournalRecordRepository> logger) : base(contextFactory)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets a journal record with specified id.
        /// </summary>
        /// <inheritdoc />
        public override async Task<JournalRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0) throw new IncorrectIdentifierException("Entity id must be positive int.");
            await using ApplicationContext context = GetContext();

            try
            {
                return await context.JournalRecords
                    .Where(record => record.Id == id)
                    .Include(record => record.Lecture)
                    .Include(record => record.Student)
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to retrieve data from database");
                throw new DataException("An error occurred while getting data from database", e);
            }
        }

        /// <summary>
        /// Gets all journal records.
        /// </summary>
        /// <inheritdoc />
        public override async Task<IEnumerable<JournalRecord>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            await using ApplicationContext context = GetContext();

            try
            {
                return await context.JournalRecords
                    .Include(record => record.Lecture)
                    .Include(record => record.Student)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to retrieve data from database");
                throw new DataException("An error occurred while getting data from database", e);
            }
        }

        /// <summary>
        /// Gets filtered collection of journal records.
        /// </summary>
        /// <inheritdoc />
        public override async Task<IEnumerable<JournalRecord>> GetFilteredAsync(CancellationToken cancellationToken = default, params Expression<Func<JournalRecord, bool>>[] predicates)
        {
            await using ApplicationContext context = GetContext();

            try
            {
                return await predicates.Aggregate<Expression<Func<JournalRecord, bool>>, IQueryable<JournalRecord>>(context.JournalRecords, 
                        (current, predicate) => current.Where(predicate))
                    .Include(record => record.Lecture)
                    .Include(record => record.Student)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to retrieve data from database");
                throw new DataException("An error occurred while getting data from database", e);
            }
        }

        /// <summary>
        /// Adds a new journal record or updates it if journal record exists.
        /// </summary>
        /// <inheritdoc />
        public override async Task<int> SaveAsync(JournalRecord entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id < 0) throw new IncorrectIdentifierException("Entity id must be non-negative int.");
            await using ApplicationContext context = GetContext();

            if (entity.Id == default)
                await context.JournalRecords.AddAsync(entity, cancellationToken);
            else
                context.JournalRecords.Update(entity);

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
        /// Deletes a journal record.
        /// </summary>
        /// <inheritdoc />
        public override async Task DeleteAsync(JournalRecord entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id <= 0) throw new IncorrectIdentifierException("Entity id must be positive int.");
            await using ApplicationContext context = GetContext();
            
            context.JournalRecords.Remove(entity);
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
                _logger.LogError(e, "Unable to delete data from database");
                throw new DataException("An error occurred while deleting data from database", e);
            }
        }
    }
}
