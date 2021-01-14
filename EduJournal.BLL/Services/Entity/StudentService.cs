#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    /// A CRUD service for students.
    /// </summary>
    public class StudentService : IStudentService
    {
        private readonly ICrudRepository<Student> _repository;
        private readonly ILogger<StudentService> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of <see cref="StudentService"/>.
        /// </summary>
        /// <param name="repository">Repository which performs CRUD operations.</param>
        /// <param name="mapper">A mapper service which converts <see cref="Student"/> instances to <see cref="StudentDto"/> and back.</param>
        /// <param name="logger">A logger.</param>
        public StudentService(ICrudRepository<Student> repository, IMapper mapper, ILogger<StudentService> logger)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<int> AddStudent(StudentDto studentDto)
        {
            if (studentDto.Id != 0) throw new IncorrectIdException("Student id must be equal to 0");

            try
            {
                Student student = _mapper.Map<Student>(studentDto);
                int id = await _repository.SaveAsync(student);
                return id;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to save a student");
                throw new UnexpectedDataException("Unable to save a student", e);
            }
        }

        /// <inheritdoc />
        public async Task<StudentDto?> GetStudent(int id)
        {
            if (id <= 0) throw new IncorrectIdException("Id must be positive number");

            try
            {
                Student? student = await _repository.GetByIdAsync(id);
                var dto = _mapper.Map<StudentDto?>(student);
                return dto;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to get a student");
                throw new UnexpectedDataException("Unable to get a student", e);
            }
        }

        /// <inheritdoc />
        public async Task<IList<StudentDto>> GetStudents()
        {
            try
            {
                IEnumerable<Student> students = await _repository.GetAllAsync();
                var list = _mapper.Map<IList<StudentDto>>(students);
                return list;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to get a student");
                throw new UnexpectedDataException("Unable to get a student", e);
            }
        }

        /// <inheritdoc />
        public async Task<IList<StudentDto>> GetStudentsByIds(IEnumerable<int> ids)
        {
            int[] idArray = ids as int[] ?? ids.ToArray();
            if (idArray.Any(id => id <= 0)) throw new IncorrectIdException("Id must be positive number");

            try
            {
                IEnumerable<Student> students = await _repository.GetFilteredAsync(CancellationToken.None,
                    student => idArray.Contains(student.Id));
                var list = _mapper.Map<IList<StudentDto>>(students);
                return list;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to get a student");
                throw new UnexpectedDataException("Unable to get a student", e);
            }
        }

        /// <inheritdoc />
        public async Task<int> UpdateStudent(StudentDto studentDto)
        {
            if (studentDto.Id <= 0) throw new IncorrectIdException("Id must be positive number");

            try
            {
                Student student = _mapper.Map<Student>(studentDto);
                int id = await _repository.SaveAsync(student);
                return id;
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Unable to update student which does not exist");
                throw;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to save a student");
                throw new UnexpectedDataException("Unable to save a student", e);
            }
        }

        /// <inheritdoc />
        public async Task DeleteStudent(int id)
        {
            if (id <= 0) throw new IncorrectIdException("Id must be positive number");
            
            try
            {
                await _repository.DeleteAsync(new Student { Id = id });
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Unable to delete student which does not exist");
                throw;
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unexpected data error");
                throw new UnexpectedDataException("Unable to save a student", e);
            }
        }
    }
}
