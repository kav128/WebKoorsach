#nullable enable
using System;
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
    /// A CRUD service for lecturers.
    /// </summary>
    public class LecturerService : ILecturerService
    {
        private readonly ICrudRepository<Lecturer> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<LecturerService> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="LecturerService"/>.
        /// </summary>
        /// <param name="repository">Repository which performs CRUD operations.</param>
        /// <param name="mapper">A mapper service which converts <see cref="Lecturer"/> instances to <see cref="LecturerDto"/> and back.</param>
        /// <param name="logger">A logger.</param>
        public LecturerService(ICrudRepository<Lecturer> repository, IMapper mapper, ILogger<LecturerService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<int> AddLecturer(LecturerDto lecturerDto)
        {
            if (lecturerDto.Id != 0) throw new IncorrectIdException("Lecturer id must be equal to 0");
            
            Lecturer lecturer = _mapper.Map<Lecturer>(lecturerDto);
            try
            {
                return await _repository.SaveAsync(lecturer);
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to save a lecturer");
                throw new UnexpectedDataException("Unable to save a lecturer", e);
            }
        }

        /// <inheritdoc />
        public async Task<LecturerDto?> GetLecturer(int id)
        {
            if (id <= 0) throw new IncorrectIdException("Id must be positive number");
            
            Lecturer? lecturer;
            try
            {
                lecturer = await _repository.GetByIdAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to get a lecturer");
                throw new UnexpectedDataException("Unable to get a lecturer", e);
            }

            var lecturerDto = _mapper.Map<LecturerDto?>(lecturer);
            return lecturerDto;
        }

        /// <inheritdoc />
        public async Task<IList<LecturerDto>> GetLecturers()
        {
            IEnumerable<Lecturer> lecturers;
            try
            {
                lecturers = await _repository.GetAllAsync();
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to get lecturers"); 
                throw new UnexpectedDataException("Unable to get lecturers", e);
            }
            var list = _mapper.Map<IList<LecturerDto>>(lecturers);
            return list;
        }

        /// <inheritdoc />
        public async Task<int> UpdateLecturer(LecturerDto lecturerDto)
        {
            if (lecturerDto.Id <= 0) throw new IncorrectIdException("Id must be positive number");
            
            Lecturer lecturer = _mapper.Map<Lecturer>(lecturerDto);
            try
            {
                int id = await _repository.SaveAsync(lecturer);
                return id;
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Unable to update lecturer which does not exist");
                throw;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to save a lecturer");
                throw new UnexpectedDataException("Unable to save a lecturer", e);
            }
        }

        /// <inheritdoc />
        public async Task DeleteLecturer(int id)
        {
            if (id <= 0) throw new IncorrectIdException("Id must be positive number");

            try
            {
                await _repository.DeleteAsync(new Lecturer { Id = id });
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Unable to delete lecturer which does not exist");
                throw;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unexpected data error");
                throw new UnexpectedDataException("Unable to save a lecturer", e);
            }
        }
    }
}
