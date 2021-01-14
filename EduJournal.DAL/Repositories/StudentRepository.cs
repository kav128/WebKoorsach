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
    /// Represents a repository of students.
    /// </summary>
    public class StudentRepository : CrudRepository<Student>
    {
        private readonly ILogger<StudentRepository> _logger;

        /// <summary>
        /// Initializes a new instance of repository of students.
        /// </summary>
        /// <param name="contextFactory">A factory for crating database context.</param>
        /// <param name="logger">An instance of logger.</param>
        public StudentRepository(IDbContextFactory<ApplicationContext> contextFactory, ILogger<StudentRepository> logger) : base(contextFactory)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets a student with specified id.
        /// </summary>
        /// <inheritdoc />
        public override async Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id < 0) throw new IncorrectIdentifierException("Entity id must be non-negative int.");
            await using ApplicationContext context = GetContext();

            try
            {
                return await context.Students
                    .Where(student => student.Id == id)
                    .Include(student => student.JournalRecords)
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to retrieve data from database");
                throw new DataException("An error occurred while getting data from database", e);
            }
        }

        /// <summary>
        /// Gets all students.
        /// </summary>
        /// <inheritdoc />
        public override async Task<IEnumerable<Student>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            await using ApplicationContext context = GetContext();

            try
            {
                return await context.Students
                    .Include(student => student.JournalRecords)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to retrieve data from database");
                throw new DataException("An error occurred while getting data from database", e);
            }
        }

        /// <summary>
        /// Gets filtered collection of students.
        /// </summary>
        /// <inheritdoc />
        public override async Task<IEnumerable<Student>> GetFilteredAsync(CancellationToken cancellationToken = default, params Expression<Func<Student, bool>>[] predicates)
        {
            await using ApplicationContext context = GetContext();

            try
            {
                return await predicates.Aggregate<Expression<Func<Student, bool>>, IQueryable<Student>>(context.Students,
                        (current, expression) => current.Where(expression))
                    .Include(student => student.JournalRecords)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to retrieve data from database");
                throw new DataException("An error occurred while getting data from database", e);
            }
        }
        
        /// <summary>
        /// Adds a new student or updates it if student exists.
        /// </summary>
        /// <inheritdoc />
        public override async Task<int> SaveAsync(Student entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id < 0) throw new IncorrectIdentifierException("Entity id must be non-negative int.");
            await using ApplicationContext context = GetContext();

            if (entity.Id == default)
                await context.Students.AddAsync(entity, cancellationToken);
            else
                context.Students.Update(entity);

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
        /// Deletes a student.
        /// </summary>
        /// <inheritdoc />
        public override async Task DeleteAsync(Student entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id <= 0) throw new IncorrectIdentifierException("Entity id must be positive int.");
            await using ApplicationContext context = GetContext();

            context.Students.Remove(entity);
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
                throw new DataException("An error occurred while delete data from database", e);
            }
        }
    }
}
