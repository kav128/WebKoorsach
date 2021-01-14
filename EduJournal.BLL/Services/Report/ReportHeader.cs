namespace EduJournal.BLL.Services.Report
{
    /// <summary>
    /// Represents a report header.
    /// </summary>
    public record ReportHeader
    {
        /// <summary>
        /// Gets or inits lecture name. Can be null.
        /// </summary>
        public string? Lecture { get; init; }

        /// <summary>
        /// Gets or inits student full name. Can be null.
        /// </summary>
        public string? Student { get; init; }

        /// <summary>
        /// Gets or inits course name.
        /// </summary>
        public string Course { get; init; }
    }
}
