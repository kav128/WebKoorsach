#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using EduJournal.BLL.DTO;
using EduJournal.DAL.Exceptions;

namespace EduJournal.BLL.Services.Entity
{
    /// <summary>
    /// A CRUD service interface for courses.
    /// </summary>
    public interface ICourseService
    {
        /// <summary>
        /// Creates a new course.
        /// </summary>
        /// <param name="courseDto">Course data.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains course id.</returns>
        /// <exception cref="IncorrectIdException">Course id does not equal to 0.</exception>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task<int> AddCourse(CourseDto courseDto);
        
        /// <summary>
        /// Gets a course with specified id.
        /// </summary>
        /// <param name="id">Course id.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains course data.</returns>
        /// <exception cref="IncorrectIdException">Course id is non-positive.</exception>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task<CourseDto?> GetCourse(int id);
        
        /// <summary>
        /// Gets all course entries.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains list of courses.</returns>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task<IList<CourseDto>> GetAll();
        
        /// <summary>
        /// Updates a course data.
        /// </summary>
        /// <param name="courseDto">Course data.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains course id.</returns>
        /// <exception cref="IncorrectIdException">Course id is non-positive.</exception>
        /// <exception cref="EntityNotFoundException">The lecturer with specified id does not exist.</exception>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task<int> UpdateCourse(CourseDto courseDto);
        
        /// <summary>
        /// Deletes course.
        /// </summary>
        /// <param name="id">Course id.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="IncorrectIdException">Course id is non-positive.</exception>
        /// <exception cref="EntityNotFoundException">The course with specified id does not exist.</exception>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task DeleteCourse(int id);
    }
}
