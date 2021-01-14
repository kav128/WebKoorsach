using System.Collections.Generic;
using System.Threading.Tasks;
using EduJournal.BLL.DTO;

namespace EduJournal.BLL.Services.Entity
{
    /// <summary>
    /// A CRUD service interface for journal records.
    /// </summary>
    public interface IJournalService
    {
        /// <summary>
        /// Saves a journal record.
        /// </summary>
        /// <param name="journalRecordDto">Journal record data.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        /// <exception cref="ReferenceNotFoundException">Specified lecturer or course is missing.</exception>
        public Task SaveRecord(JournalRecordDto journalRecordDto);
        
        /// <summary>
        /// Gets journal records with specified filters.
        /// </summary>
        /// <param name="lectureId">Lecture id. Optional parameter. Cannot be combined with <see cref="courseId"/>.</param>
        /// <param name="studentId">Student id. Optional parameter.</param>
        /// <param name="courseId">Course id. Optional parameter. Cannot be combined with <see cref="lectureId"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains list of journal records.</returns>
        /// <exception cref="UnexpectedDataException">Database returned error or unexpected behavior.</exception>
        public Task<IList<JournalRecordDto>> GetRecords(int lectureId = default, int studentId = default, int courseId = default);
    }
}
