#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using EduJournal.BLL.DTO;
using EduJournal.DAL.Exceptions;

namespace EduJournal.BLL.Services.Entity
{
    /// <summary>
    /// A CRUD service interface for students.
    /// </summary>
    public interface IStudentService
    {
        /// <summary>
        /// Creates a new student.
        /// </summary>
        /// <param name="studentDto">Student data.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains student id.</returns>
        /// <exception cref="IncorrectIdException">Student id does not equal to 0.</exception>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task<int> AddStudent(StudentDto studentDto);
        
        /// <summary>
        /// Gets a student with specified id.
        /// </summary>
        /// <param name="id">Student id.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains student data.</returns>
        /// <exception cref="IncorrectIdException">Student id is non-positive.</exception>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task<StudentDto?> GetStudent(int id);
        
        /// <summary>
        /// Gets all student entries.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains list of students.</returns>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task<IList<StudentDto>> GetStudents();
        
        /// <summary>
        /// Gets all student entries.
        /// </summary>
        /// <param name="ids">Student identifier.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains list of students.</returns>
        /// <exception cref="IncorrectIdException">At least one student id is non-positive.</exception>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task<IList<StudentDto>> GetStudentsByIds(IEnumerable<int> ids);
        
        /// <summary>
        /// Updates a student data.
        /// </summary>
        /// <param name="studentDto">Student data.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains student id.</returns>
        /// <exception cref="IncorrectIdException">Student id is non-positive.</exception>
        /// <exception cref="EntityNotFoundException">The lecturer with specified id does not exist.</exception>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task<int> UpdateStudent(StudentDto studentDto);

        /// <summary>
        /// Deletes student.
        /// </summary>
        /// <param name="id">Student id.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="IncorrectIdException">Student id is non-positive.</exception>
        /// <exception cref="EntityNotFoundException">The student with specified id does not exist.</exception>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        Task DeleteStudent(int id);
    }
}
