using System;

namespace EduJournal.BLL.Services.Report.Formats
{
    /// <summary>
    /// Represents a container of report formatters.
    /// </summary>
    public interface IFormatterManager
    {
        /// <summary>
        /// Saves a new instance of <see cref="IReportFormatter"/> with specified name.
        /// </summary>
        /// <param name="name">Formatter name.</param>
        /// <param name="formatter">Formatter instance.</param>
        /// <exception cref="ArgumentNullException"><see cref="name"/> or <see cref="formatter"/> is null.</exception>
        /// <exception cref="RegistrationNotSupportedException">Internal formatter container is read-only.</exception>
        /// <exception cref="DuplicatedFormatterException">Formatter with specified name does already exist.</exception>
        public void RegisterFormatter(string name, IReportFormatter formatter);

        /// <summary>
        /// Gets registered formatter.
        /// </summary>
        /// <param name="name">Formatter name.</param>
        /// <returns>Formatter.</returns>
        /// <exception cref="FormatterNotFoundException">Formatter with specified name does not exist.</exception>
        public IReportFormatter GetFormatter(string name);
    }
}
