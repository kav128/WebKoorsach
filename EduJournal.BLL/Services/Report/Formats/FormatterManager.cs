using System;
using System.Collections.Generic;

namespace EduJournal.BLL.Services.Report.Formats
{
    /// <summary>
    /// Represents a container of report formatters.
    /// </summary>
    public class FormatterManager : IFormatterManager
    {
        private readonly IDictionary<string, IReportFormatter> _formatterStorage;

        /// <summary>
        /// Initializes a new instance of <see cref="FormatterManager"/> with default storage.
        /// </summary>
        public FormatterManager() : this(new Dictionary<string, IReportFormatter>())
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FormatterManager"/> with custom storage.
        /// </summary>
        /// <param name="formatterStorage">A key-value storage that contains formatters.</param>
        /// <exception cref="ArgumentNullException"><see cref="formatterStorage"/> is null.</exception>
        public FormatterManager(IDictionary<string, IReportFormatter> formatterStorage)
        {
            _formatterStorage = formatterStorage ?? throw new ArgumentNullException(nameof(formatterStorage));
        }

        /// <summary>
        /// Saves a new instance of <see cref="IReportFormatter"/> with specified name.
        /// </summary>
        /// <param name="name">Formatter name.</param>
        /// <param name="formatter">Formatter instance.</param>
        /// <exception cref="ArgumentNullException"><see cref="name"/> or <see cref="formatter"/> is null.</exception>
        /// <exception cref="RegistrationNotSupportedException">Internal formatter container is read-only.</exception>
        /// <exception cref="DuplicatedFormatterException">Formatter with specified name does already exist.</exception>
        public void RegisterFormatter(string name, IReportFormatter formatter)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            
            try
            {
                _formatterStorage.Add(name, formatter);
            }
            catch (NotSupportedException e)
            {
                throw new RegistrationNotSupportedException("The storage does not support registering new formatters.", e);
            }
            catch (ArgumentException e)
            {
                throw new DuplicatedFormatterException("Formatter with such name is already registered.", e);
            }
        }
        
        /// <summary>
        /// Gets registered formatter.
        /// </summary>
        /// <param name="name">Formatter name.</param>
        /// <returns>Formatter.</returns>
        /// <exception cref="FormatterNotFoundException">Formatter with specified name does not exist.</exception>
        public IReportFormatter GetFormatter(string name)
        {
            try
            {
                return _formatterStorage[name];
            }
            catch (KeyNotFoundException e)
            {
                throw new FormatterNotFoundException("Unable to found specified formatter.", e);
            }
        }
    }
}
