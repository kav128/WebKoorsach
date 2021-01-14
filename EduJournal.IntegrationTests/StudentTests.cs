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
    public class StudentTests
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
            var response = await _client.GetAsync("/Student");

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
                await context.Students.AddRangeAsync(
                    new Student { FullName = "Test 1" },
                    new Student { FullName = "Test 2" },
                    new Student { FullName = "Test 3" });
                await context.SaveChangesAsync();
            }
            
            // Act
            var response = await _client.GetAsync("/Student");

            // Assert 
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<IList<StudentModel>>();
            
            Assert.That(content, Is.EquivalentTo(new StudentModel[]
            {
                new(1, "Test 1"),
                new(2, "Test 2"),
                new(3, "Test 3")
            }));
        }

        [Test]
        public async Task GetSingleTest()
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Students.AddAsync(new Student { FullName = "Test" });
                await context.SaveChangesAsync();
            }
            
            // Act
            var response = await _client.GetAsync("/Student/1");
            var model = await response.Content.ReadFromJsonAsync<StudentModel>();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.That(model, Is.EqualTo(new StudentModel(1, "Test")));
        }

        [Test]
        public async Task PostTest()
        {
            var model = new StudentAddModel("Test");
            
            // Act
            var response = await _client.PostAsync("/Student", JsonContent.Create(model));
            
            // Assert
            response.EnsureSuccessStatusCode();
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                Student student = await context.Students.FirstAsync();
                Assert.That(student, Is.EqualTo(new Student { Id = 1, FullName = "Test" }));
            }
        }

        [Test]
        public async Task PostTest_WrongFormat()
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test", Email = "Test" });
                await context.SaveChangesAsync();
            }

            var model = new StudentAddModel(null!);
            
            // Act
            var response = await _client.PostAsync("/Student", JsonContent.Create(model));
            
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task PatchTest()
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Students.AddAsync(new Student { FullName = "Test" });
                await context.SaveChangesAsync();
            }
            var model = new StudentUpdateModel(1, "Test After");
            
            // Act
            var response = await _client.PatchAsync("/Student", JsonContent.Create(model));
            
            // Assert
            response.EnsureSuccessStatusCode();
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                Student student = await context.Students.FirstAsync();
                Assert.That(student, Is.EqualTo(new Student { Id = 1, FullName = "Test After" }));
            }
        }

        [TestCase(1, null)]
        [TestCase(0, "Test")]
        public async Task PatchTest_WrongFormat(int id, string name)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Students.AddAsync(new Student { FullName = "Test" });
                await context.SaveChangesAsync();
            }
            var model = new StudentUpdateModel(id, name);
            
            // Act
            var response = await _client.PatchAsync("/Student", JsonContent.Create(model));
            
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task DeleteTest()
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Students.AddAsync(new Student { Id = 1, FullName = "Test" });
                await context.SaveChangesAsync();
            }
            
            // Act
            var response = await _client.DeleteAsync("/Student?id=1");
            
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
                await context.Students.AddAsync(new Student { Id = 1, FullName = "Test" });
                await context.SaveChangesAsync();
            }
            
            // Act
            var response = await _client.DeleteAsync($"/Student?id={id}");
            
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
