#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using EduJournal.BLL.DTO;
using EduJournal.DAL.Exceptions;

namespace EduJournal.BLL.Services.Entity
{
    /// <summary>
    /// A CRUD service interface for lectures.
    /// </summary>
    public interface ILectureService
    {
        /// <summary>
        /// Creates a new lecture.
        /// </summary>
        /// <param name="lectureDto">lecture data.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains lecture id.</returns>
        /// <exception cref="IncorrectIdException">Lecture id does not equal to 0.</exception>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task<int> AddLecture(LectureDto lectureDto);
        
        /// <summary>
        /// Gets a lecture with specified id.
        /// </summary>
        /// <param name="id">Lecture id.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains lecture data.</returns>
        /// <exception cref="IncorrectIdException">Lecture id is non-positive.</exception>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task<LectureDto?> GetLecture(int id);
        
        /// <summary>
        /// Gets all lecture entries.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains list of lectures.</returns>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task<IList<LectureDto>> GetAll();
        
        /// <summary>
        /// Gets all lecture entries in specified course.
        /// </summary>
        /// <param name="courseId">Course id.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains list of lectures.</returns>
        /// <exception cref="IncorrectIdException">Course id is non-positive.</exception>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task<IList<LectureDto>> GetAllByCourse(int courseId);
        
        /// <summary>
        /// Updates a lecture data.
        /// </summary>
        /// <param name="lectureDto">Lecture data.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains lecture id.</returns>
        /// <exception cref="IncorrectIdException">Lecture id is non-positive.</exception>
        /// <exception cref="EntityNotFoundException">The lecturer with specified id does not exist.</exception>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task<int> UpdateLecture(LectureDto lectureDto);
        
        /// <summary>
        /// Deletes lecture.
        /// </summary>
        /// <param name="id">Lecture id.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="IncorrectIdException">Lecture id is non-positive.</exception>
        /// <exception cref="EntityNotFoundException">The lecture with specified id does not exist.</exception>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task DeleteLecture(int id);
    }
}
