using EduJournal.DAL.Entities;
using EduJournal.DAL.Repositories;
using EduJournal.DAL.Repositories.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EduJournal.DAL
{
    /// <summary>
    /// Extension methods for setting up DAL services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class DataAccessLayerDI
    {
        /// <summary>
        /// Registers DAL services in <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="connectionString">SQL Server connection string.</param>
        /// <param name="loggerFactory">Logger factory.</param>
        /// <param name="logSensitiveData">Specifies whether to log data transferring in SQL queries.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services,
            string connectionString, ILoggerFactory? loggerFactory, bool logSensitiveData = false)
        {
            services.AddDbContextFactory<ApplicationContext>(builder =>
            {
                builder = builder.UseSqlServer(connectionString);
                if (loggerFactory != null) builder.UseLoggerFactory(loggerFactory);
                if (logSensitiveData) builder.EnableSensitiveDataLogging();
            });
            
            services.AddScoped<ICrudRepository<Course>, CourseRepository>();
            services.AddScoped<ICrudRepository<JournalRecord>, JournalRecordRepository>();
            services.AddScoped<ICrudRepository<Lecturer>, LecturerRepository>();
            services.AddScoped<ICrudRepository<Lecture>, LectureRepository>();
            services.AddScoped<ICrudRepository<Student>, StudentRepository>();
            return services;
        }

        /// <summary>
        /// Registers DAL services in <see cref="IServiceCollection"/> for testing purposes.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns></returns>
        public static IServiceCollection AddTestingDataAccessLayer(this IServiceCollection services)
        {
            return services.AddScoped<ICrudRepository<Course>, CourseRepository>()
                .AddScoped<ICrudRepository<JournalRecord>, JournalRecordRepository>()
                .AddScoped<ICrudRepository<Lecturer>, LecturerRepository>()
                .AddScoped<ICrudRepository<Lecture>, LectureRepository>()
                .AddScoped<ICrudRepository<Student>, StudentRepository>();
        }
    }
}
