using Microsoft.EntityFrameworkCore;

namespace EduJournal.DAL.Tests
{
    public class ApplicationFakeContext : ApplicationContext
    {
        public ApplicationFakeContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
