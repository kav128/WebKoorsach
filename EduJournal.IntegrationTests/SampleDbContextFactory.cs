using System;
using System.Data.Common;
using EduJournal.DAL;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EduJournal.IntegrationTests
{
    public class SampleDbContextFactory : IDbContextFactory<ApplicationContext>, IDisposable
    {
        private DbConnection? _connection;

        private DbContextOptions<ApplicationContext> CreateOptions() =>
            new DbContextOptionsBuilder<ApplicationContext>()
                .UseSqlite(_connection!)
                .Options;

        public bool SimulateSqlException { get; set; }

        public ApplicationContext CreateDbContext()
        {
            if (_connection != null && !SimulateSqlException) return new ApplicationFakeContext(CreateOptions());
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            DbContextOptions<ApplicationContext> options = CreateOptions();
            using var context = new ApplicationFakeContext(options);
            if (SimulateSqlException)
                context.Database.EnsureDeleted();
            else
                context.Database.EnsureCreated();

            return new ApplicationFakeContext(CreateOptions());
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            
            if (_connection == null) return;
            _connection.Dispose();
            _connection = null;
        }
    }
}
