using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EduJournal.DAL;
using EduJournal.DAL.Entities;
using EduJournal.Presentation.Web;
using EduJournal.Presentation.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace EduJournal.IntegrationTests
{
    public class JournalTests
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

        public static IEnumerable<IEnumerable<object>> GetTestCaseGenerator()
        {
            var models = new JournalRecordModel[]
            {
                new(false, 0, 1, 1),
                new(true, 4, 2, 1),
                new(true, 3, 1, 2),
                new(false, 0, 2, 2),
                new(false, 0, 1, 3),
                new(true, 5, 2, 3),
                new(true, 1, 1, 4),
                new(false, 0, 1, 5),
                new(true, 0, 1, 6)
            };

            yield return new object[] { "", models };
            yield return new object[] { "?lectureId=1", models.Where(model => model.LectureId == 1).ToArray() };
            yield return new object[] { "?studentId=1", models.Where(model => model.StudentId == 1).ToArray() };
            yield return new object[] { "?studentId=2", models.Where(model => model.StudentId == 2).ToArray() };
            yield return new object[] { "?courseId=1", models.Where(model => new[] { 1, 2, 3 }.Contains(model.LectureId)).ToArray() };
            yield return new object[] { "?courseId=2", models.Where(model => new[] { 4, 5, 6 }.Contains(model.LectureId)).ToArray() };
            yield return new object[] { "?courseId=1&studentId=2", models.Where(model => new[] { 1, 2, 3 }.Contains(model.LectureId) && model.StudentId == 2).ToArray() };
            yield return new object[] { "?lectureId=1&studentId=2", models.Where(model => model.LectureId == 1 && model.StudentId == 2).ToArray() };
        }

        [TestCaseSource(nameof(GetTestCaseGenerator))]
        public async Task GetTest(string queryString, IList<JournalRecordModel> models)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test Lecturer" });
                await context.SaveChangesAsync();
                await context.Courses.AddRangeAsync(
                    new Course { Name = "Test Course 1", LecturerId = 1 },
                    new Course { Name = "Test Course 2", LecturerId = 1 });
                await context.SaveChangesAsync();
                await context.Lectures.AddRangeAsync(
                    new Lecture { Name = "Test Lecture 1 1", CourseId = 1 },
                    new Lecture { Name = "Test Lecture 2 1", CourseId = 1 },
                    new Lecture { Name = "Test Lecture 3 1", CourseId = 1 },
                    new Lecture { Name = "Test Lecture 1 2", CourseId = 2 },
                    new Lecture { Name = "Test Lecture 2 2", CourseId = 2 },
                    new Lecture { Name = "Test Lecture 3 2", CourseId = 2 });
                await context.SaveChangesAsync();
                await context.Students.AddRangeAsync(
                    new Student { FullName = "Test Student 1" },
                    new Student { FullName = "Test Student 2" });
                await context.SaveChangesAsync();
                await context.JournalRecords.AddRangeAsync(
                    new JournalRecord { LectureId = 1, StudentId = 1, Attendance = false, Score = 0 },
                    new JournalRecord { LectureId = 1, StudentId = 2, Attendance = true, Score = 4 },
                    new JournalRecord { LectureId = 2, StudentId = 1, Attendance = true, Score = 3 },
                    new JournalRecord { LectureId = 2, StudentId = 2, Attendance = false, Score = 0 },
                    new JournalRecord { LectureId = 3, StudentId = 1, Attendance = false, Score = 0 },
                    new JournalRecord { LectureId = 3, StudentId = 2, Attendance = true, Score = 5 },
                    new JournalRecord { LectureId = 4, StudentId = 1, Attendance = true, Score = 1 },
                    new JournalRecord { LectureId = 5, StudentId = 1, Attendance = false, Score = 0 },
                    new JournalRecord { LectureId = 6, StudentId = 1, Attendance = true, Score = 0 });
                await context.SaveChangesAsync();
            }
            
            // Act
            var response = await _client.GetAsync($"/Journal{queryString}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<IList<JournalRecordModel>>();
            Assert.That(content, Is.EquivalentTo(models));
        }

        [Test]
        public async Task PostTest()
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test Lecturer" });
                await context.SaveChangesAsync();
                await context.Courses.AddRangeAsync(
                    new Course { Name = "Test Course 1", LecturerId = 1 },
                    new Course { Name = "Test Course 2", LecturerId = 1 });
                await context.SaveChangesAsync();
                await context.Lectures.AddRangeAsync(
                    new Lecture { Name = "Test Lecture 1 1", CourseId = 1 },
                    new Lecture { Name = "Test Lecture 2 1", CourseId = 1 },
                    new Lecture { Name = "Test Lecture 3 1", CourseId = 1 },
                    new Lecture { Name = "Test Lecture 1 2", CourseId = 2 },
                    new Lecture { Name = "Test Lecture 2 2", CourseId = 2 },
                    new Lecture { Name = "Test Lecture 3 2", CourseId = 2 });
                await context.SaveChangesAsync();
                await context.Students.AddRangeAsync(
                    new Student { FullName = "Test Student 1" },
                    new Student { FullName = "Test Student 2" });
                await context.SaveChangesAsync();
            }

            // Act
            var model = new JournalRecordModel(false, 0, 1, 1);
            var response = await _client.PostAsJsonAsync("/Journal", model);

            // Assert
            response.EnsureSuccessStatusCode();
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                JournalRecord record = await context.JournalRecords.FirstAsync();
                Assert.That(record, Is.EqualTo(new JournalRecord { Id = 1, Attendance = false, Score = 0, StudentId = 1, LectureId = 1 }));
            }
        }

        [Test]
        [TestCase(false, 1, 1, 1)]
        [TestCase(false, -1, 1, 1)]
        [TestCase(true, -1, 1, 1)]
        [TestCase(true, 9, 1, 1)]
        [TestCase(true, 3, 0, 1)]
        [TestCase(true, 3, 1, 0)]
        [TestCase(true, 3, 1, 0)]
        [TestCase(true, 3, 0, 0)]
        public async Task PostTest_WrongData(bool attendance, int score, int studentId, int lectureId)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test Lecturer" });
                await context.SaveChangesAsync();
                await context.Courses.AddRangeAsync(
                    new Course { Name = "Test Course 1", LecturerId = 1 },
                    new Course { Name = "Test Course 2", LecturerId = 1 });
                await context.SaveChangesAsync();
                await context.Lectures.AddRangeAsync(
                    new Lecture { Name = "Test Lecture 1 1", CourseId = 1 },
                    new Lecture { Name = "Test Lecture 2 1", CourseId = 1 },
                    new Lecture { Name = "Test Lecture 3 1", CourseId = 1 },
                    new Lecture { Name = "Test Lecture 1 2", CourseId = 2 },
                    new Lecture { Name = "Test Lecture 2 2", CourseId = 2 },
                    new Lecture { Name = "Test Lecture 3 2", CourseId = 2 });
                await context.SaveChangesAsync();
                await context.Students.AddRangeAsync(
                    new Student { FullName = "Test Student 1" },
                    new Student { FullName = "Test Student 2" });
                await context.SaveChangesAsync();
            }

            // Act
            var model = new JournalRecordModel(attendance, score, studentId, lectureId);
            var response = await _client.PostAsJsonAsync("/Journal", model);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
