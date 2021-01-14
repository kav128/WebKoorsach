using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EduJournal.BLL.Services.Report;
using EduJournal.DAL;
using EduJournal.DAL.Entities;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;
using EduJournal.Presentation.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace EduJournal.IntegrationTests
{
    public class ReportTests
    {
        private TestServer _testServer;
        private ApplicationContext _dbContext;
        private SampleDbContextFactory _contextFactory;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _contextFactory = new SampleDbContextFactory();

            var webHostBuilder = new WebHostBuilder()
                .ConfigureServices(collection => collection.AddSingleton<IDbContextFactory<ApplicationContext>>(_contextFactory))
                .UseEnvironment("Testing")
                .UseStartup<Startup>();

            _testServer = new TestServer(webHostBuilder);
            _dbContext = _testServer.Services.GetService<IDbContextFactory<ApplicationContext>>()!.CreateDbContext();
            _dbContext.Database.EnsureCreated();
            _client = _testServer.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _dbContext.Dispose();
            _contextFactory.Dispose();
            _testServer.Dispose();
        }

        public static IEnumerable<IEnumerable<object>> JsonGetTestCaseGenerator()
        {
            yield return new object[]
            {
                "/Report/student.json?studentId=1&courseId=1",
                new ReportData
                {
                    Header = new()
                    {
                        Student = "Test Student 1",
                        Course = "Test Course 1"
                    },
                    Records = new []
                    {
                        new ReportRecord { Lecture = "Test Lecture 1 1", Attendance = false, Score = 0 },
                        new ReportRecord { Lecture = "Test Lecture 2 1", Attendance = true, Score = 3 },
                        new ReportRecord { Lecture = "Test Lecture 3 1", Attendance = false, Score = 0 }
                    },
                    AttendancePercentage = 100D / 3D,
                    AverageScore = 1D
                }
            };

            yield return new object[]
            {
                "/Report/lecture.json?lectureId=1",
                new ReportData
                {
                    Header = new()
                    {
                        Lecture = "Test Lecture 1 1",
                        Course = "Test Course 1"
                    },
                    Records = new[]
                    {
                        new ReportRecord { Student = "Test Student 1", Attendance = false, Score = 0 },
                        new ReportRecord { Student = "Test Student 2", Attendance = true, Score = 4 },
                    },
                    AttendancePercentage = 50D
                }
            };
        }

        [TestCaseSource(nameof(JsonGetTestCaseGenerator))]
        public async Task JsonGetTest(string requestUri, ReportData expected)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test Lecturer" });
                await context.SaveChangesAsync();
                await context.Courses.AddRangeAsync(
                    new Course { Id = 1, Name = "Test Course 1", LecturerId = 1 },
                    new Course { Id = 2, Name = "Test Course 2", LecturerId = 1 });
                await context.SaveChangesAsync();
                await context.Lectures.AddRangeAsync(
                    new Lecture { Id = 1, Name = "Test Lecture 1 1", CourseId = 1 },
                    new Lecture { Id = 2, Name = "Test Lecture 2 1", CourseId = 1 },
                    new Lecture { Id = 3, Name = "Test Lecture 3 1", CourseId = 1 },
                    new Lecture { Id = 4, Name = "Test Lecture 1 2", CourseId = 2 },
                    new Lecture { Id = 5, Name = "Test Lecture 2 2", CourseId = 2 },
                    new Lecture { Id = 6, Name = "Test Lecture 3 2", CourseId = 2 });
                await context.SaveChangesAsync();
                await context.Students.AddRangeAsync(
                    new Student { Id = 1, FullName = "Test Student 1" },
                    new Student { Id = 2, FullName = "Test Student 2" });
                await context.SaveChangesAsync();
                await context.JournalRecords.AddRangeAsync(
                    new JournalRecord { Id = 1, LectureId = 1, StudentId = 1, Attendance = false, Score = 0 },
                    new JournalRecord { Id = 2, LectureId = 1, StudentId = 2, Attendance = true, Score = 4 },
                    new JournalRecord { Id = 3, LectureId = 2, StudentId = 1, Attendance = true, Score = 3 },
                    new JournalRecord { Id = 4, LectureId = 2, StudentId = 2, Attendance = false, Score = 0 },
                    new JournalRecord { Id = 5, LectureId = 3, StudentId = 1, Attendance = false, Score = 0 },
                    new JournalRecord { Id = 6, LectureId = 3, StudentId = 2, Attendance = true, Score = 5 },
                    new JournalRecord { Id = 7, LectureId = 4, StudentId = 1, Attendance = true, Score = 1 },
                    new JournalRecord { Id = 8, LectureId = 5, StudentId = 1, Attendance = false, Score = 0 },
                    new JournalRecord { Id = 9, LectureId = 6, StudentId = 1, Attendance = true, Score = 0 });
                await context.SaveChangesAsync();
            }

            // Act
            var response = await _client.GetAsync(requestUri);

            // Assert
            response.EnsureSuccessStatusCode();
            var reportData = await response.Content.ReadFromJsonAsync<ReportData>();
            Assert.That(reportData, Is.EqualTo(expected));
        }
        
        public static IEnumerable<IEnumerable<object>> XmlGetTestCaseGenerator()
        {
            yield return new object[]
            {
                "/Report/student.xml?studentId=1&courseId=1",
                new ReportData
                {
                    Header = new()
                    {
                        Student = "Test Student 1",
                        Course = "Test Course 1"
                    },
                    Records = new []
                    {
                        new ReportRecord { Lecture = "Test Lecture 1 1", Attendance = false, Score = 0 },
                        new ReportRecord { Lecture = "Test Lecture 2 1", Attendance = true, Score = 3 },
                        new ReportRecord { Lecture = "Test Lecture 3 1", Attendance = false, Score = 0 }
                    },
                    AttendancePercentage = 100D / 3D,
                    AverageScore = 1D
                }
            };

            yield return new object[]
            {
                "/Report/lecture.xml?lectureId=1",
                new ReportData
                {
                    Header = new()
                    {
                        Lecture = "Test Lecture 1 1",
                        Course = "Test Course 1"
                    },
                    Records = new[]
                    {
                        new ReportRecord { Student = "Test Student 1", Attendance = false, Score = 0 },
                        new ReportRecord { Student = "Test Student 2", Attendance = true, Score = 4 },
                    },
                    AttendancePercentage = 50D
                }
            };
        }
        
        [TestCaseSource(nameof(XmlGetTestCaseGenerator))]
        public async Task XmlGetTest(string requestUri, ReportData expected)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test Lecturer" });
                await context.SaveChangesAsync();
                await context.Courses.AddRangeAsync(
                    new Course { Id = 1, Name = "Test Course 1", LecturerId = 1 },
                    new Course { Id = 2, Name = "Test Course 2", LecturerId = 1 });
                await context.SaveChangesAsync();
                await context.Lectures.AddRangeAsync(
                    new Lecture { Id = 1, Name = "Test Lecture 1 1", CourseId = 1 },
                    new Lecture { Id = 2, Name = "Test Lecture 2 1", CourseId = 1 },
                    new Lecture { Id = 3, Name = "Test Lecture 3 1", CourseId = 1 },
                    new Lecture { Id = 4, Name = "Test Lecture 1 2", CourseId = 2 },
                    new Lecture { Id = 5, Name = "Test Lecture 2 2", CourseId = 2 },
                    new Lecture { Id = 6, Name = "Test Lecture 3 2", CourseId = 2 });
                await context.SaveChangesAsync();
                await context.Students.AddRangeAsync(
                    new Student { Id = 1, FullName = "Test Student 1" },
                    new Student { Id = 2, FullName = "Test Student 2" });
                await context.SaveChangesAsync();
                await context.JournalRecords.AddRangeAsync(
                    new JournalRecord { Id = 1, LectureId = 1, StudentId = 1, Attendance = false, Score = 0 },
                    new JournalRecord { Id = 2, LectureId = 1, StudentId = 2, Attendance = true, Score = 4 },
                    new JournalRecord { Id = 3, LectureId = 2, StudentId = 1, Attendance = true, Score = 3 },
                    new JournalRecord { Id = 4, LectureId = 2, StudentId = 2, Attendance = false, Score = 0 },
                    new JournalRecord { Id = 5, LectureId = 3, StudentId = 1, Attendance = false, Score = 0 },
                    new JournalRecord { Id = 6, LectureId = 3, StudentId = 2, Attendance = true, Score = 5 },
                    new JournalRecord { Id = 7, LectureId = 4, StudentId = 1, Attendance = true, Score = 1 },
                    new JournalRecord { Id = 8, LectureId = 5, StudentId = 1, Attendance = false, Score = 0 },
                    new JournalRecord { Id = 9, LectureId = 6, StudentId = 1, Attendance = true, Score = 0 });
                await context.SaveChangesAsync();
            }

            // Act
            var response = await _client.GetAsync(requestUri);

            // Assert
            response.EnsureSuccessStatusCode();
            await using var reportStream = await response.Content.ReadAsStreamAsync();
            IExtendedXmlSerializer serializer = new ConfigurationContainer()
                .EnableImplicitTypingFromAll<ReportData>()
                .Type<ReportData>().Name("Report")
                .Type<ReportRecord>().Name("Record")
                .Create();
            var reportData = serializer.Deserialize<ReportData>(reportStream);
            Assert.That(reportData, Is.EqualTo(expected));
        }
    }
}
