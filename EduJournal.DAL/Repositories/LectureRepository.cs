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
    /// Represents a repository of lectures.
    /// </summary>
    public class LectureRepository : CrudRepository<Lecture>
    {
        private readonly ILogger<LectureRepository> _logger;

        /// <summary>
        /// Initializes a new instance of repository of lectures.
        /// </summary>
        /// <param name="contextFactory">A factory for creating database context.</param>
        /// <param name="logger">An instance of logger.</param>
        public LectureRepository(IDbContextFactory<ApplicationContext> contextFactory, ILogger<LectureRepository> logger) : base(contextFactory)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets a lecture with specified id.
        /// </summary>
        /// <inheritdoc />
        public override async Task<Lecture?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0) throw new IncorrectIdentifierException("Entity id must be positive int.");
            await using ApplicationContext context = GetContext();

            try
            {
                return await context.Lectures.Where(lecture => lecture.Id == id)
                    .Include(lecture => lecture.Course)
                    .Include(lecture => lecture.JournalRecords)
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to retrieve data from database");
                throw new DataException("An error occurred while getting data from database", e);
            }
        }

        /// <summary>
        /// Gets all lectures.
        /// </summary>
        /// <inheritdoc />
        public override async Task<IEnumerable<Lecture>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            await using ApplicationContext context = GetContext();

            try
            {
                return await context.Lectures
                    .Include(lecture => lecture.Course)
                    .Include(lecture => lecture.JournalRecords)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to retrieve data from database");
                throw new DataException("An error occurred while getting data from database", e);
            }
        }

        /// <summary>
        /// Gets filtered collection of lectures.
        /// </summary>
        /// <inheritdoc />
        public override async Task<IEnumerable<Lecture>> GetFilteredAsync(CancellationToken cancellationToken = default, params Expression<Func<Lecture, bool>>[] predicates)
        {
            await using ApplicationContext context = GetContext();

            try
            {
                return await predicates.Aggregate<Expression<Func<Lecture, bool>>, IQueryable<Lecture>>(context.Lectures,
                        (current, expression) => current.Where(expression))
                    .Include(lecture => lecture.Course)
                    .Include(lecture => lecture.JournalRecords)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to retrieve data from database");
                throw new DataException("An error occurred while getting data from database", e);
            }
        }
        
        /// <summary>
        /// Adds a new lecture or updates it if lecture exists.
        /// </summary>
        /// <inheritdoc />
        public override async Task<int> SaveAsync(Lecture entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id < 0) throw new IncorrectIdentifierException("Entity id must be non-negative int.");
            await using ApplicationContext context = GetContext();

            if (entity.Id == default)
                await context.Lectures.AddAsync(entity, cancellationToken);
            else
            {
                context.Lectures.Update(entity);
                context.Entry(entity).Property(lecture => lecture.CourseId).IsModified = false;
            }

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
        /// Deletes a lecture.
        /// </summary>
        /// <inheritdoc />
        public override async Task DeleteAsync(Lecture entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id <= 0) throw new IncorrectIdentifierException("Entity id must be positive int.");
            await using ApplicationContext context = GetContext();
            
            context.Lectures.Remove(entity);
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
