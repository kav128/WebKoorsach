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
    /// A CRUD service for lectures.
    /// </summary>
    public class LectureService : ILectureService
    {
        private readonly ICrudRepository<Lecture> _repository;
        private readonly ILogger<LectureService> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of <see cref="LectureService"/>.
        /// </summary>
        /// <param name="repository">Repository which performs CRUD operations.</param>
        /// <param name="mapper">A mapper service which converts <see cref="Lecture"/> instances to <see cref="LectureDto"/> and back.</param>
        /// <param name="logger">A logger.</param>
        public LectureService(ICrudRepository<Lecture> repository, IMapper mapper, ILogger<LectureService> logger)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<int> AddLecture(LectureDto lectureDto)
        {
            if (lectureDto.Id != 0) throw new IncorrectIdException("Lecture id must be equal to 0");

            try
            {
                Lecture lecture = _mapper.Map<Lecture>(lectureDto);
                int id = await _repository.SaveAsync(lecture);
                return id;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to save a lecture");
                throw new UnexpectedDataException("Unable to save a lecture", e);
            }
        }

        /// <inheritdoc />
        public async Task<LectureDto?> GetLecture(int id)
        {
            if (id <= 0) throw new IncorrectIdException("Id must be positive number");

            try
            {
                Lecture? lecture = await _repository.GetByIdAsync(id);
                var dto = _mapper.Map<LectureDto?>(lecture);
                return dto;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to get a lecture");
                throw new UnexpectedDataException("Unable to get a lecture", e);
            }
        }

        /// <inheritdoc />
        public async Task<IList<LectureDto>> GetAll()
        {
            try
            {
                IEnumerable<Lecture> lectures = await _repository.GetAllAsync();
                var list = _mapper.Map<IList<LectureDto>>(lectures);
                return list;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to get lectures");
                throw new UnexpectedDataException("Unable to get lectures", e);
            }
        }

        /// <inheritdoc />
        public async Task<IList<LectureDto>> GetAllByCourse(int courseId)
        {
            if (courseId <= 0) throw new IncorrectIdException("Id must be positive number");

            try
            {
                IEnumerable<Lecture> lectures = await _repository.GetFilteredAsync(default,
                    lecture => lecture.CourseId == courseId);
                var list = _mapper.Map<IList<LectureDto>>(lectures);
                return list;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to get lectures");
                throw new UnexpectedDataException("Unable to get lectures", e);
            }
        }

        /// <inheritdoc />
        public async Task<int> UpdateLecture(LectureDto lectureDto)
        {
            if (lectureDto.Id <= 0) throw new IncorrectIdException("Id must be positive number");

            try
            {
                Lecture lecture = _mapper.Map<Lecture>(lectureDto);
                int id = await _repository.SaveAsync(lecture);
                return id;
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Unable to update lecture which does not exist");
                throw;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to save a lecture");
                throw new UnexpectedDataException("Unable to save a lecture", e);
            }
        }

        /// <inheritdoc />
        public async Task DeleteLecture(int id)
        {
            if (id <= 0) throw new IncorrectIdException("Id must be positive number");

            try
            {
                await _repository.DeleteAsync(new Lecture { Id = id });
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Unable to delete lecture which does not exist");
                throw;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unexpected data error");
                throw new UnexpectedDataException("Unable to save a lecture", e);
            }
        }
    }
}
