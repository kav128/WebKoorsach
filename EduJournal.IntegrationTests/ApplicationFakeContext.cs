using EduJournal.DAL;
using Microsoft.EntityFrameworkCore;

namespace EduJournal.IntegrationTests
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
