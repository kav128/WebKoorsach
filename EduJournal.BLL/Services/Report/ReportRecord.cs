namespace EduJournal.BLL.Services.Report
{
    /// <summary>
    /// Represents a record in report that contains data about one lecture.
    /// </summary>
    public record ReportRecord
    {
        /// <summary>
        /// Gets or inits student's full name. Can be null.
        /// </summary>
        public string? Student { get; init; }
        
        /// <summary>
        /// Gets or inits lecture's name. Can be null.
        /// </summary>
        public string? Lecture { get; init; }
        
        /// <summary>
        /// Gets or inits attendance.
        /// </summary>
        public bool Attendance { get; init; }
        
        /// <summary>
        /// Gets or inits student's score.
        /// </summary>
        public int Score { get; init; }
    }
}
