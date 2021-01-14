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
    public class CourseTests
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
            var response = await _client.GetAsync("/Course");

            // Assert 
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<IList<CourseModel>>();
            
            Assert.That(content, Is.EquivalentTo(Array.Empty<CourseModel>()));
        }
        
        [Test]
        public async Task GetTest()
        {
            // Arrange// Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test", Email = "Test" });
                await context.SaveChangesAsync();
                await context.Courses.AddRangeAsync(
                    new Course { LecturerId = 1, Name = "Test 1" },
                    new Course { LecturerId = 1, Name = "Test 2" },
                    new Course { LecturerId = 1, Name = "Test 3" });
                await context.SaveChangesAsync();
            }
            
            // Act
            var response = await _client.GetAsync("/Course");

            // Assert 
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<IList<CourseModel>>();
            
            Assert.That(content, Is.EquivalentTo(new CourseModel[]
            {
                new(1, "Test 1", 1, Array.Empty<int>()),
                new(2, "Test 2", 1, Array.Empty<int>()),
                new(3, "Test 3", 1, Array.Empty<int>())
            }));
        }

        [Test]
        public async Task GetSingleTest()
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test", Email = "Test" });
                await context.SaveChangesAsync();
                await context.Courses.AddAsync(new Course { Id = 1, LecturerId = 1, Name = "Test" });
                await context.SaveChangesAsync();
            }
            
            // Act
            var response = await _client.GetAsync("/Course/1");
            var model = await response.Content.ReadFromJsonAsync<CourseModel>();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.That(model, Is.EqualTo(new CourseModel(1, "Test", 1, Array.Empty<int>())));
        }

        [Test]
        public async Task PostTest()
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test", Email = "Test" });
                await context.SaveChangesAsync();
            }
            var model = new CourseAddModel("Test", 1);
            
            // Act
            var response = await _client.PostAsync("/Course", JsonContent.Create(model));
            
            // Assert
            response.EnsureSuccessStatusCode();
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                Course course = await context.Courses.FirstAsync();
                Assert.That(course, Is.EqualTo(new Course { Id = 1, LecturerId = 1, Name = "Test", Lectures = Array.Empty<Lecture>() }));
            }
        }

        [TestCase(null, 1)]
        [TestCase(null, 0)]
        [TestCase("Test", 0)]
        public async Task PostTest_WrongFormat(string name, int lecturerId)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test", Email = "Test" });
                await context.SaveChangesAsync();
            }

            var model = new CourseAddModel(name!, lecturerId);
            
            // Act
            var response = await _client.PostAsync("/Course", JsonContent.Create(model));
            
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task PatchTest()
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test", Email = "Test" });
                await context.SaveChangesAsync();
                await context.Courses.AddAsync(new Course { Id = 1, LecturerId = 1, Name = "Test Before" });
                await context.SaveChangesAsync();
            }
            var model = new CourseUpdateModel(1, "Test After", 1);
            
            // Act
            var response = await _client.PatchAsync("/Course", JsonContent.Create(model));
            
            // Assert
            response.EnsureSuccessStatusCode();
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                Course course = await context.Courses.FirstAsync();
                Assert.That(course, Is.EqualTo(new Course { Id = 1, LecturerId = 1, Name = "Test After", Lectures = Array.Empty<Lecture>() }));
            }
        }

        [TestCase(1, null, 1)]
        [TestCase(1, null, 0)]
        [TestCase(1, "Test", 0)]
        [TestCase(0, "Test", 1)]
        [TestCase(-1, "Test", 1)]
        public async Task PatchTest_WrongFormat(int id, string name, int lecturerId)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test", Email = "Test" });
                await context.SaveChangesAsync();
                await context.Courses.AddAsync(new Course { Id = 1, LecturerId = 1, Name = "Test Before" });
                await context.SaveChangesAsync();
            }
            var model = new CourseUpdateModel(id, name, lecturerId);
            
            // Act
            var response = await _client.PatchAsync("/Course", JsonContent.Create(model));
            
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task DeleteTest()
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test", Email = "Test" });
                await context.SaveChangesAsync();
                await context.Courses.AddAsync(new Course { Id = 1, LecturerId = 1, Name = "Test" });
                await context.SaveChangesAsync();
            }
            
            // Act
            var response = await _client.DeleteAsync("/Course?id=1");
            
            // Assert
            response.EnsureSuccessStatusCode();
            Assert.That(_dbContext.Courses.IsEmpty());
        }

        [Test]
        public async Task DeleteTest_WrongData([Values(-1, 0)] int id)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test", Email = "Test" });
                await context.SaveChangesAsync();
                await context.Courses.AddAsync(new Course { Id = 1, LecturerId = 1, Name = "Test" });
                await context.SaveChangesAsync();
            }
            
            // Act
            var response = await _client.DeleteAsync($"/Course?id={id}");
            
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
