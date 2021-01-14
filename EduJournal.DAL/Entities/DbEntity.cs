namespace EduJournal.DAL.Entities
{
    /// <summary>
    /// Represents an abstract generic entity.
    /// </summary>
    public abstract record DbEntity
    {
        /// <summary>
        /// Gets or inits identifier.
        /// </summary>
        public int Id { get; init; }
    }
}
