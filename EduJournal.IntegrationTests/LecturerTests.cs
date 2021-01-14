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
    public class LecturerTests
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
            var response = await _client.GetAsync("/Lecturer");

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
                await context.Lecturers.AddRangeAsync(
                    new Lecturer { FullName = "Test 1" },
                    new Lecturer { FullName = "Test 2" },
                    new Lecturer { FullName = "Test 3" });
                await context.SaveChangesAsync();
            }
            
            // Act
            var response = await _client.GetAsync("/Lecturer");

            // Assert 
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<IList<LecturerModel>>();
            
            Assert.That(content, Is.EquivalentTo(new LecturerModel[]
            {
                new(1, "Test 1", Array.Empty<int>()),
                new(2, "Test 2", Array.Empty<int>()),
                new(3, "Test 3", Array.Empty<int>())
            }));
        }

        [Test]
        public async Task GetSingleTest()
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test" });
                await context.SaveChangesAsync();
            }
            
            // Act
            var response = await _client.GetAsync("/Lecturer/1");
            var model = await response.Content.ReadFromJsonAsync<LecturerModel>();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.That(model, Is.EqualTo(new LecturerModel(1, "Test", Array.Empty<int>())));
        }

        [Test]
        public async Task PostTest()
        {
            var model = new LecturerAddModel("Test");
            
            // Act
            var response = await _client.PostAsync("/Lecturer", JsonContent.Create(model));
            
            // Assert
            response.EnsureSuccessStatusCode();
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                Lecturer lecturer = await context.Lecturers.Include(l => l.Courses).FirstAsync();
                Assert.That(lecturer, Is.EqualTo(new Lecturer { Id = 1, FullName = "Test", Courses = Array.Empty<Course>() }));
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

            var model = new LecturerAddModel(null!);
            
            // Act
            var response = await _client.PostAsync("/Lecturer", JsonContent.Create(model));
            
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task PatchTest()
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test" });
                await context.SaveChangesAsync();
            }
            var model = new LecturerUpdateModel(1, "Test After");
            
            // Act
            var response = await _client.PatchAsync("/Lecturer", JsonContent.Create(model));
            
            // Assert
            response.EnsureSuccessStatusCode();
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                Lecturer lecturer = await context.Lecturers.Include(l => l.Courses).FirstAsync();
                Assert.That(lecturer, Is.EqualTo(new Lecturer { Id = 1, FullName = "Test After", Courses = Array.Empty<Course>() }));
            }
        }
        
        [TestCase(1, null)]
        [TestCase(0, "Test")]
        public async Task PatchTest_WrongFormat(int id, string name)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { FullName = "Test" });
                await context.SaveChangesAsync();
            }
            var model = new LecturerUpdateModel(id, name);
            
            // Act
            var response = await _client.PatchAsync("/Lecturer", JsonContent.Create(model));
            
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task DeleteTest()
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddAsync(new Lecturer { Id = 1, FullName = "Test" });
                await context.SaveChangesAsync();
            }
            
            // Act
            var response = await _client.DeleteAsync("/Lecturer?id=1");
            
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
                await context.Lecturers.AddAsync(new Lecturer { Id = 1, FullName = "Test" });
                await context.SaveChangesAsync();
            }
            
            // Act
            var response = await _client.DeleteAsync($"/Lecturer?id={id}");
            
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
