using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EduJournal.DAL.Entities;
using EduJournal.DAL.Exceptions;
using EduJournal.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EduJournal.DAL.Tests
{
    [TestFixture]
    public class CourseRepositoryTests
    {
        private SampleDbContextFactory _contextFactory = null!;
        
        [SetUp]
        public void Setup()
        {
            _contextFactory = new SampleDbContextFactory();
            using ApplicationContext context = _contextFactory.CreateDbContext();
            context.Lecturers.Add(new Lecturer { FullName = "Test Lecturer" });
            context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _contextFactory.Dispose();
        }

        #region GetById

        public static IEnumerable<IEnumerable<object?>> GetByIdAsyncTestCaseGenerator()
        {
            yield return new object?[] { Array.Empty<Course>(), 1, null };
            
            var courses = new Course[]
            {
                new() { Name = "Course 1", Lectures = Enumerable.Empty<Lecture>(), LecturerId = 1 },
                new() { Name = "Course 2", Lectures = Enumerable.Empty<Lecture>(), LecturerId = 1 },
                new() { Name = "Course 3", Lectures = Enumerable.Empty<Lecture>(), LecturerId = 1 }
            };
            for (var i = 0; i < courses.Length; i++)
                yield return new object[] { courses, i + 1, courses[i] with { Id = i + 1 } };
        }

        [TestCaseSource(nameof(GetByIdAsyncTestCaseGenerator))]
        public async Task GetByIdAsyncTest(IEnumerable<Course> data, int id, Course? expected)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Courses.AddRangeAsync(data);
                await context.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<CourseRepository>>();
            var repository = new CourseRepository(_contextFactory, loggerMock.Object);
            
            // Act
            Course? course = await repository.GetByIdAsync(id);
            
            // Assert
            Assert.That(course, Is.EqualTo(expected));
        }

        [Test]
        public void GetByIdAsyncTest_ThrowsException([Values(1)] int id)
        {
            // Arrange
            var loggerMock = new Mock<ILogger<CourseRepository>>();
            var repository = new CourseRepository(_contextFactory, loggerMock.Object);
            
            // Act
            _contextFactory.SimulateSqlException = true;
            async Task TestAction() => await repository.GetByIdAsync(id);

            // Assert
            Assert.ThrowsAsync<DataException>(TestAction);
        }

        [Test]
        public void GetByIdAsyncTest_ThrowsDataException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<CourseRepository>>();
            var repository = new CourseRepository(_contextFactory, loggerMock.Object);
            
            // Act
            _contextFactory.SimulateSqlException = true;
            async Task TestAction() => await repository.DeleteAsync(new Course { Id = 1 });

            // Assert
            Assert.ThrowsAsync<DataException>(TestAction);
        }


        #endregion

        #region GetAll

        public static IEnumerable<IEnumerable<object>> GetAllAsyncTestCaseGenerator()
        {
            var courses = new Course[]
            {
                new() { Name = "Course 1", Lectures = Enumerable.Empty<Lecture>(), LecturerId = 1 },
                new() { Name = "Course 2", Lectures = Enumerable.Empty<Lecture>(), LecturerId = 1 },
                new() { Name = "Course 3", Lectures = Enumerable.Empty<Lecture>(), LecturerId = 1 }
            };

            yield return new[] { courses, courses.Select((course, index) => course with { Id = index + 1 }) };
        }

        [TestCaseSource(nameof(GetAllAsyncTestCaseGenerator))]
        public async Task GetAllAsyncTest(IEnumerable<Course> data, IEnumerable<Course> expected)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Courses.AddRangeAsync(data);
                await context.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<CourseRepository>>();
            var repository = new CourseRepository(_contextFactory, loggerMock.Object);

            // Act
            IEnumerable<Course> courses = await repository.GetAllAsync();
            
            // Assert
            Assert.That(courses, Is.EquivalentTo(expected));
        }
        
        [Test]
        public void GetAllAsyncTest_ThrowsException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<CourseRepository>>();
            var repository = new CourseRepository(_contextFactory, loggerMock.Object);
            
            // Act
            _contextFactory.SimulateSqlException = true;
            async Task TestAction() => await repository.GetAllAsync();

            // Assert
            Assert.ThrowsAsync<DataException>(TestAction);
        }

        #endregion

        #region GetFiltered

        public static IEnumerable<IEnumerable<object>> GetFilteredAsyncTestCaseGenerator()
        {
            var courses = new Course[]
            {
                new() { Name = "Course 1", Lectures = Enumerable.Empty<Lecture>(), LecturerId = 1 },
                new() { Name = "Course 2", Lectures = Enumerable.Empty<Lecture>(), LecturerId = 1 },
                new() { Name = "Course 3", Lectures = Enumerable.Empty<Lecture>(), LecturerId = 1 }
            };

            yield return new object[] { courses, Array.Empty<Expression<Func<Course, bool>>>(), courses };
            
            yield return new object[]
            {
                courses,
                new Expression<Func<Course, bool>>[] { course => course.Name == "Course 2" },
                courses.Where(course => course.Name == "Course 2")
            };
        }

        [TestCaseSource(nameof(GetFilteredAsyncTestCaseGenerator))]
        public async Task GetFilteredAsyncTest(IEnumerable<Course> data, Expression<Func<Course, bool>>[] expressions, IEnumerable<Course> expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.Courses.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<CourseRepository>>();
            var repository = new CourseRepository(_contextFactory, loggerMock.Object);

            // Act
            await using var ctx2 = _contextFactory.CreateDbContext();
            await ctx2.Database.CloseConnectionAsync();
            IEnumerable<Course> filtered = await repository.GetFilteredAsync(default, expressions);

            // Assert
            Assert.That(filtered, Is.EquivalentTo(expected));
        }
        
        [Test]
        public void GetFilteredAsyncTest_ThrowsException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<CourseRepository>>();
            var repository = new CourseRepository(_contextFactory, loggerMock.Object);
            
            // Act
            _contextFactory.SimulateSqlException = true;
            async Task TestAction() => await repository.GetFilteredAsync();

            // Assert
            Assert.ThrowsAsync<DataException>(TestAction);
        }

        #endregion

        #region Save

        public static IEnumerable<IEnumerable<object>> SaveAsyncTestCaseGenerator()
        {
            static IEnumerable<object> GetInsertCase(IEnumerable<string> names, string name, int expectedId )
            {
                var course = new Course { Name = name, Lectures = Enumerable.Empty<Lecture>(), LecturerId = 1 };
                IEnumerable<Course> data = names.Select(s => new Course
                {
                    Name = s,
                    Lectures = Enumerable.Empty<Lecture>(),
                    LecturerId = 1
                });
                return new object[]
                {
                    data,
                    course,
                    data.Select((std, index) => std with { Id = index + 1}).Append(course with { Id = expectedId })
                };
            }

            yield return GetInsertCase(Enumerable.Empty<string>(), "Test Course", 1);
            yield return GetInsertCase(new[] { "Course 1", "Course 2", "Course 3" }, "Course 4", 4);

            yield return new object[]
            {
                new[] { "Course 1", "Course 2", "Course 3" }.Select(s => new Course
                {
                    Name = s,
                    Lectures = Enumerable.Empty<Lecture>(),
                    LecturerId = 1
                }),
                new Course { Id = 2, Name = "Course 2 Changed", Lectures = Enumerable.Empty<Lecture>(), LecturerId = 1 },
                new[] { "Course 1", "Course 2 Changed", "Course 3" }.Select((s, i) => new Course
                {
                    Id = i + 1,
                    Name = s,
                    Lectures = Enumerable.Empty<Lecture>(),
                    LecturerId = 1
                })
            };
        }

        [TestCaseSource(nameof(SaveAsyncTestCaseGenerator))]
        public async Task SaveAsyncTest(IEnumerable<Course> data, Course entity, IEnumerable<Course> expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.Courses.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<CourseRepository>>();
            var repository = new CourseRepository(_contextFactory, loggerMock.Object);

            // Act
            await repository.SaveAsync(entity);

            // Assert
            await using ApplicationContext ctx2 = _contextFactory.CreateDbContext();
            List<Course> courses = await ctx2.Courses.Include(course => course.Lectures).ToListAsync();
            Assert.That(courses, Is.EqualTo(expected));
        }

        [Test]
        public void SaveAsyncTest_ThrowsDataException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<CourseRepository>>();
            var repository = new CourseRepository(_contextFactory, loggerMock.Object);
            
            // Act
            _contextFactory.SimulateSqlException = true;
            async Task TestAction() => await repository.SaveAsync(new Course { Name = "Test" });

            // Assert
            Assert.ThrowsAsync<DataException>(TestAction);
        }
        
        [Test]
        public void SaveAsyncTest_ThrowsEntityNotFoundException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<CourseRepository>>();
            var repository = new CourseRepository(_contextFactory, loggerMock.Object);
            
            // Act
            async Task TestAction() => await repository.SaveAsync(new Course { Id = 1, Name = "Test" });

            // Assert
            Assert.ThrowsAsync<EntityNotFoundException>(TestAction);
        }
        
        [Test]
        public void SaveAsyncTest_ThrowsIncorrectIdentifierException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<CourseRepository>>();
            var repository = new CourseRepository(_contextFactory, loggerMock.Object);
            
            // Act
            async Task TestAction() => await repository.SaveAsync(new Course { Id = -1, Name = "Test" });

            // Assert
            Assert.ThrowsAsync<IncorrectIdentifierException>(TestAction);
        }
        
        #endregion

        #region Delete

        public static IEnumerable<IEnumerable<object>> DeleteAsyncTestCaseGenerator()
        {
            static IEnumerable<object> GetTestCase(IEnumerable<string> names, int id)
            {
                IEnumerable<Course> data = names.Select(s => new Course
                {
                    Name = s,
                    Lectures = Enumerable.Empty<Lecture>(),
                    LecturerId = 1
                });
                return new object[]
                {
                    data,
                    new Course { Id = id, Name = "", Lectures = Enumerable.Empty<Lecture>() },
                    data.Select((std, index) => std with { Id = index + 1}).Where(course => course.Id != id)
                };
            }

            yield return GetTestCase(new[] { "Course 1", "Course 2", "Course 3" }, 2);
        }

        [TestCaseSource(nameof(DeleteAsyncTestCaseGenerator))]
        public async Task DeleteAsyncTest(IEnumerable<Course> data, Course entity, IEnumerable<Course> expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.Courses.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<CourseRepository>>();
            var repository = new CourseRepository(_contextFactory, loggerMock.Object);

            // Act
            await repository.DeleteAsync(entity);

            // Assert
            await using ApplicationContext ctx2 = _contextFactory.CreateDbContext();
            List<Course> courses = await ctx2.Courses.Include(course => course.Lectures).ToListAsync();
            Assert.That(courses, Is.EqualTo(expected));
        }

        public static IEnumerable<IEnumerable<object>> DeleteAsyncExceptionTestCaseGenerator()
        {
            static IEnumerable<object> GetTestCase(IEnumerable<string> names, int id)
            {
                IEnumerable<Course> data = names.Select(s => new Course
                {
                    Name = s,
                    Lectures = Enumerable.Empty<Lecture>(),
                    LecturerId = 1
                });
                return new object[]
                {
                    data,
                    new Course { Id = id, Name = "", Lectures = Enumerable.Empty<Lecture>() }
                };
            }

            yield return GetTestCase(new[] { "Course 1", "Course 2", "Course 3" }, 5)
                .Append(typeof(EntityNotFoundException))
                .ToArray();
        }

        [TestCaseSource(nameof(DeleteAsyncExceptionTestCaseGenerator))]
        public async Task DeleteAsyncTest_ThrowsException(IEnumerable<Course> data, Course entity, Type expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.Courses.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<CourseRepository>>();
            var repository = new CourseRepository(_contextFactory, loggerMock.Object);

            // Act

            // Assert
            Assert.ThrowsAsync(expected, async () => await repository.DeleteAsync(entity));
        }
        
        [Test]
        public void DeleteAsyncTest_ThrowsIncorrectIdentifierException([Values(-1, 0)] int id)
        {
            // Arrange
            var loggerMock = new Mock<ILogger<CourseRepository>>();
            var repository = new CourseRepository(_contextFactory, loggerMock.Object);
            
            // Act
            async Task TestAction() => await repository.DeleteAsync(new Course { Id = id, Name = "Test" });

            // Assert
            Assert.ThrowsAsync<IncorrectIdentifierException>(TestAction);
        }
        
        #endregion
    }
}
