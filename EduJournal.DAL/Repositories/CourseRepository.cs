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
    /// Represents a repository of courses.
    /// </summary>
    public class CourseRepository : CrudRepository<Course>
    {
        private readonly ILogger<CourseRepository> _logger;

        /// <summary>
        /// Initializes a new instance of repository of courses.
        /// </summary>
        /// <param name="contextFactory">A factory for creating database context.</param>
        /// <param name="logger">An instance of logger.</param>
        public CourseRepository(IDbContextFactory<ApplicationContext> contextFactory, ILogger<CourseRepository> logger) : base(contextFactory)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets a course with specified id.
        /// </summary>
        /// <inheritdoc />
        public override async Task<Course?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0) throw new IncorrectIdentifierException("Entity id must be positive int.");
            await using ApplicationContext context = GetContext();

            try
            {
                return await context.Courses
                    .Where(course => course.Id == id)
                    .Include(course => course.Lecturer)
                    .Include(course => course.Lectures)
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to retrieve data from database");
                throw new DataException("An error occurred while getting data from database", e);
            }
        }

        /// <summary>
        /// Gets all courses.
        /// </summary>
        /// <inheritdoc />
        public override async Task<IEnumerable<Course>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            await using ApplicationContext context = GetContext();

            try
            {
                return await context.Courses
                    .Include(course => course.Lecturer)
                    .Include(course => course.Lectures)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to retrieve data from database");
                throw new DataException("An error occurred while getting data from database", e);
            }
        }

        /// <summary>
        /// Gets filtered collection of courses.
        /// </summary>
        /// <inheritdoc />
        public override async Task<IEnumerable<Course>> GetFilteredAsync(CancellationToken cancellationToken = default, params Expression<Func<Course, bool>>[] predicates)
        {
            await using ApplicationContext context = GetContext();

            try
            {
                return await predicates.Aggregate<Expression<Func<Course, bool>>, IQueryable<Course>>(context.Courses, 
                        (current, predicate) => current.Where(predicate))
                    .Include(course => course.Lecturer)
                    .Include(course => course.Lectures)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to retrieve data from database");
                throw new DataException("An error occurred while getting data from database", e);
            }
        }

        /// <summary>
        /// Adds a new course or updates it if course exists.
        /// </summary>
        /// <inheritdoc />
        public override async Task<int> SaveAsync(Course entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id < 0) throw new IncorrectIdentifierException("Entity id must be non-negative int.");
            await using ApplicationContext context = GetContext();

            if (entity.Id == default)
                await context.Courses.AddAsync(entity, cancellationToken);
            else
                context.Courses.Update(entity);

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
        /// Deletes a course.
        /// </summary>
        /// <inheritdoc />
        public override async Task DeleteAsync(Course entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id <= 0) throw new IncorrectIdentifierException("Entity id must be positive int.");
            await using ApplicationContext context = GetContext();
            
            context.Courses.Remove(entity);
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
