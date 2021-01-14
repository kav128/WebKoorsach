using System;
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
    public class LectureTests
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
        
        [Test]
        public async Task GetTest_Empty()
        {
            // Arrange
            
            // Act
            var response = await _client.GetAsync("/Lecture");

            // Assert 
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<IList<LectureModel>>();
            
            Assert.That(content, Is.EquivalentTo(Array.Empty<LectureModel>()));
        }
        
        [Test]
        public async Task GetTest()
        {
            // Arrange// Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test Lecturer" });
                await context.SaveChangesAsync();
                await context.Courses.AddAsync(new Course { Name = "Test Course", LecturerId = 1 });
                await context.SaveChangesAsync();
                await context.Lectures.AddRangeAsync(
                    new Lecture { Name = "Test 1", CourseId = 1 },
                    new Lecture { Name = "Test 2", CourseId = 1 },
                    new Lecture { Name = "Test 3", CourseId = 1 });
                await context.SaveChangesAsync();
            }
            
            // Act
            var response = await _client.GetAsync("/Lecture");

            // Assert 
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<IList<LectureModel>>();
            
            Assert.That(content, Is.EquivalentTo(new LectureModel[]
            {
                new(1, "Test 1", 1),
                new(2, "Test 2", 1),
                new(3, "Test 3", 1)
            }));
        }

        [Test]
        public async Task GetSingleTest()
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test Lecturer" });
                await context.SaveChangesAsync();
                await context.Courses.AddAsync(new Course { Name = "Test Course", LecturerId = 1 });
                await context.SaveChangesAsync();
                await context.Lectures.AddRangeAsync(
                    new Lecture { Name = "Test 1", CourseId = 1 },
                    new Lecture { Name = "Test 2", CourseId = 1 },
                    new Lecture { Name = "Test 3", CourseId = 1 });
                await context.SaveChangesAsync();
            }
            
            // Act
            var response = await _client.GetAsync("/Lecture/1");
            var model = await response.Content.ReadFromJsonAsync<LectureModel>();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.That(model, Is.EqualTo(new LectureModel(1, "Test 1", 1)));
        }

        [Test]
        public async Task PostTest()
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test Lecturer" });
                await context.SaveChangesAsync();
                await context.Courses.AddAsync(new Course { Name = "Test Course", LecturerId = 1 });
                await context.SaveChangesAsync();
            }
            var model = new LectureAddModel("Test", 1);
            
            // Act
            var response = await _client.PostAsync("/Lecture", JsonContent.Create(model));
            
            // Assert
            response.EnsureSuccessStatusCode();
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                Lecture lecture = await context.Lectures.Include(l => l.JournalRecords).FirstAsync();
                Assert.That(lecture, Is.EqualTo(new Lecture
                    {
                        Id = 1,
                        Name = "Test",
                        CourseId = 1,
                        Course = await context.Courses.FirstAsync(c => c.Id == 1),
                        JournalRecords = Array.Empty<JournalRecord>()
                    }));
            }
        }

        [TestCase(null, 1)]
        [TestCase(null, 0)]
        [TestCase("Test", 0)]
        public async Task PostTest_WrongFormat(string name, int courseId)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test", Email = "Test" });
                await context.SaveChangesAsync();
            }

            var model = new LectureAddModel(name, courseId);
            
            // Act
            var response = await _client.PostAsync("/Lecture", JsonContent.Create(model));
            
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task PatchTest()
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test Lecturer" });
                await context.SaveChangesAsync();
                await context.Courses.AddAsync(new Course { Name = "Test Course", LecturerId = 1 });
                await context.SaveChangesAsync();
                await context.Lectures.AddAsync(new Lecture { Name = "Test Before", CourseId = 1 });
                await context.SaveChangesAsync();
            }
            var model = new LectureUpdateModel(1, "Test After");
            
            // Act
            var response = await _client.PatchAsync("/Lecture", JsonContent.Create(model));
            
            // Assert
            response.EnsureSuccessStatusCode();
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                Lecture lecture = await context.Lectures.FirstAsync();
                Assert.That(lecture, Is.EqualTo(new Lecture { Id = 1, Name = "Test After", CourseId = 1 }));
            }
        }
        
        [TestCase(0, null!)]
        [TestCase(1, null!)]
        [TestCase(0, "Test")]
        public async Task PatchTest_WrongFormat(int id, string name)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test Lecturer" });
                await context.SaveChangesAsync();
                await context.Courses.AddAsync(new Course { Name = "Test Course", LecturerId = 1 });
                await context.SaveChangesAsync();
                await context.Lectures.AddAsync(new Lecture { Name = "Test Before", CourseId = 1 });
                await context.SaveChangesAsync();
            }
            var model = new LectureUpdateModel(id, name);
            
            // Act
            var response = await _client.PatchAsync("/Lecture", JsonContent.Create(model));
            
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task DeleteTest()
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test Lecturer" });
                await context.SaveChangesAsync();
                await context.Courses.AddAsync(new Course { Name = "Test Course", LecturerId = 1 });
                await context.SaveChangesAsync();
                await context.Lectures.AddAsync(new Lecture { Name = "Test Before", CourseId = 1 });
                await context.SaveChangesAsync();
            }
            
            // Act
            var response = await _client.DeleteAsync("/Lecture?id=1");
            
            // Assert
            response.EnsureSuccessStatusCode();
            Assert.That(_dbContext.Lectures.IsEmpty());
        }

        [Test]
        public async Task DeleteTest_WrongData([Values(-1, 0)] int id)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test Lecturer" });
                await context.SaveChangesAsync();
                await context.Courses.AddAsync(new Course { Name = "Test Course", LecturerId = 1 });
                await context.SaveChangesAsync();
                await context.Lectures.AddAsync(new Lecture { Name = "Test Before", CourseId = 1 });
                await context.SaveChangesAsync();
            }
            
            // Act
            var response = await _client.DeleteAsync($"/Lecture?id={id}");
            
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
