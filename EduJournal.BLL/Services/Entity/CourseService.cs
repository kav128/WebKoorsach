#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EduJournal.BLL.DTO;
using EduJournal.DAL.Entities;
using EduJournal.DAL.Exceptions;
using EduJournal.DAL.Repositories.Generic;
using Microsoft.Extensions.Logging;

namespace EduJournal.BLL.Services.Entity
{
    /// <summary>
    /// A CRUD service for courses.
    /// </summary>
    public class CourseService : ICourseService
    {
        private readonly ICrudRepository<Course> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<CourseService> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="CourseService"/>.
        /// </summary>
        /// <param name="repository">Repository which performs CRUD operations.</param>
        /// <param name="mapper">A mapper service which converts <see cref="Course"/> instances to <see cref="CourseDto"/> and back.</param>
        /// <param name="logger">A logger.</param>
        public CourseService(ICrudRepository<Course> repository, IMapper mapper, ILogger<CourseService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<int> AddCourse(CourseDto courseDto)
        {
            if (courseDto.Id != 0) throw new IncorrectIdException("Course id must be equal to 0");
            
            try
            {
                var course = _mapper.Map<Course>(courseDto);
                int id = await _repository.SaveAsync(course);
                return id;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to save a course");
                throw new UnexpectedDataException("Unable to save a course", e);
            }
        }

        /// <inheritdoc />
        public async Task<CourseDto?> GetCourse(int id)
        {
            if (id <= 0) throw new IncorrectIdException("Id must be positive number");

            try
            {
                Course? course = await _repository.GetByIdAsync(id);
                var courseDto = _mapper.Map<CourseDto?>(course);
                return courseDto;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to get a course");
                throw new UnexpectedDataException("Unable to get a course", e);
            }
        }

        /// <inheritdoc />
        public async Task<IList<CourseDto>> GetAll()
        {
            try
            {
                IEnumerable<Course> courses = await _repository.GetAllAsync();
                var list = _mapper.Map<IList<CourseDto>>(courses);
                return list;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to get courses");
                throw new UnexpectedDataException("Unable to get courses", e);
            }
        }

        /// <inheritdoc />
        public async Task<int> UpdateCourse(CourseDto courseDto)
        {
            if (courseDto.Id <= 0) throw new IncorrectIdException("Id must be positive number");
            
            try
            {
                var course = _mapper.Map<Course>(courseDto);
                int id = await _repository.SaveAsync(course);
                return id;
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Unable to update course which does not exist");
                throw;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to save a course");
                throw new UnexpectedDataException("Unable to save a course", e);
            }
        }

        /// <inheritdoc />
        public async Task DeleteCourse(int id)
        {
            if (id <= 0) throw new IncorrectIdException("Id must be positive number");
            
            try
            {
                await _repository.DeleteAsync(new Course { Id = id });
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Unable to delete course which does not exist");
                throw;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unexpected data error");
                throw new UnexpectedDataException("Unable to save a course", e);
            }
        }
    }
}
