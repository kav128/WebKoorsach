#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EduJournal.BLL.DTO;
using EduJournal.DAL.Exceptions;

namespace EduJournal.BLL.Services.Entity
{
    /// <summary>
    /// A CRUD service interface for lecturers.
    /// </summary>
    public interface ILecturerService
    {
        /// <summary>
        /// Creates a new lecturer.
        /// </summary>
        /// <param name="lecturerDto">lecturer data.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains lecturer id.</returns>
        /// <exception cref="IncorrectIdException">lecturer id does not equal to 0.</exception>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task<int> AddLecturer(LecturerDto lecturerDto);
        
        /// <summary>
        /// Gets a lecturer with specified id.
        /// </summary>
        /// <param name="id">lecturer id.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains lecturer data.</returns>
        /// <exception cref="IncorrectIdException">lecturer id is non-positive.</exception>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task<LecturerDto?> GetLecturer(int id);
        
        /// <summary>
        /// Gets all lecturer entries.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains list of lecturers.</returns>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task<IList<LecturerDto>> GetLecturers();
        
        /// <summary>
        /// Updates a lecturer data.
        /// </summary>
        /// <param name="lecturerDto">lecturer data.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains lecturer id.</returns>
        /// <exception cref="IncorrectIdException">lecturer id is non-positive.</exception>
        /// <exception cref="EntityNotFoundException">The lecturer with specified id does not exist.</exception>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task<int> UpdateLecturer(LecturerDto lecturerDto);

        /// <summary>
        /// Deletes lecturer.
        /// </summary>
        /// <param name="id">lecturer id.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="IncorrectIdException">lecturer id is non-positive.</exception>
        /// <exception cref="EntityNotFoundException">The lecturer with specified id does not exist.</exception>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task DeleteLecturer(int id);
    }
}
