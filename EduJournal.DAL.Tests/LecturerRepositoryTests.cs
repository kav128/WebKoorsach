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
    public class LecturerRepositoryTests
    {
        private SampleDbContextFactory _contextFactory = null!;
        
        [SetUp]
        public void Setup()
        {
            _contextFactory = new SampleDbContextFactory();
        }

        [TearDown]
        public void TearDown() => _contextFactory.Dispose();

        #region GetById

        public static IEnumerable<IEnumerable<object?>> GetByIdAsyncTestCaseGenerator()
        {
            yield return new object?[] { Array.Empty<Lecturer>(), 1, null };
            
            var lecturers = new Lecturer[]
            {
                new() { FullName = "Lecturer 1", Courses = Enumerable.Empty<Course>() },
                new() { FullName = "Lecturer 2", Courses = Enumerable.Empty<Course>() },
                new() { FullName = "Lecturer 3", Courses = Enumerable.Empty<Course>() }
            };
            for (var i = 0; i < lecturers.Length; i++)
                yield return new object[] { lecturers, i + 1, lecturers[i] with { Id = i + 1 } };
        }

        [TestCaseSource(nameof(GetByIdAsyncTestCaseGenerator))]
        public async Task GetByIdAsyncTest(IEnumerable<Lecturer> data, int id, Lecturer? expected)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddRangeAsync(data);
                await context.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<LecturerRepository>>();
            var repository = new LecturerRepository(_contextFactory, loggerMock.Object);
            
            // Act
            Lecturer? lecturer = await repository.GetByIdAsync(id);
            
            // Assert
            Assert.That(lecturer, Is.EqualTo(expected));
        }

        [Test]
        public void GetByIdAsyncTest_ThrowsException([Values(1)] int id)
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LecturerRepository>>();
            var repository = new LecturerRepository(_contextFactory, loggerMock.Object);
            
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
            var loggerMock = new Mock<ILogger<LecturerRepository>>();
            var repository = new LecturerRepository(_contextFactory, loggerMock.Object);
            
            // Act
            _contextFactory.SimulateSqlException = true;
            async Task TestAction() => await repository.DeleteAsync(new Lecturer { Id = 1 });

            // Assert
            Assert.ThrowsAsync<DataException>(TestAction);
        }


        #endregion

        #region GetAll

        public static IEnumerable<IEnumerable<object>> GetAllAsyncTestCaseGenerator()
        {
            var lecturers = new Lecturer[]
            {
                new() { FullName = "Lecturer 1", Courses = Array.Empty<Course>() },
                new() { FullName = "Lecturer 2", Courses = Array.Empty<Course>() },
                new() { FullName = "Lecturer 3", Courses = Array.Empty<Course>() }
            };

            yield return new[] { lecturers, lecturers.Select((lecturer, index) => lecturer with { Id = index + 1 }) };
        }

        [TestCaseSource(nameof(GetAllAsyncTestCaseGenerator))]
        public async Task GetAllAsyncTest(IEnumerable<Lecturer> data, IEnumerable<Lecturer> expected)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lecturers.AddRangeAsync(data);
                await context.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<LecturerRepository>>();
            var repository = new LecturerRepository(_contextFactory, loggerMock.Object);

            // Act
            IEnumerable<Lecturer> lecturers = await repository.GetAllAsync();
            
            // Assert
            Assert.That(lecturers, Is.EquivalentTo(expected));
        }
        
        [Test]
        public void GetAllAsyncTest_ThrowsException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LecturerRepository>>();
            var repository = new LecturerRepository(_contextFactory, loggerMock.Object);
            
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
            var lecturers = new Lecturer[]
            {
                new() { FullName = "Lecturer 1", Courses = Array.Empty<Course>() },
                new() { FullName = "Lecturer 2", Courses = Array.Empty<Course>() },
                new() { FullName = "Lecturer 3", Courses = Array.Empty<Course>() }
            };

            yield return new object[] { lecturers, Array.Empty<Expression<Func<Lecturer, bool>>>(), lecturers };
            
            yield return new object[]
            {
                lecturers,
                new Expression<Func<Lecturer, bool>>[] { lecturer => lecturer.FullName == "Lecturer 2" },
                lecturers.Where(lecturer => lecturer.FullName == "Lecturer 2")
            };
        }

        [TestCaseSource(nameof(GetFilteredAsyncTestCaseGenerator))]
        public async Task GetFilteredAsyncTest(IEnumerable<Lecturer> data, Expression<Func<Lecturer, bool>>[] expressions, IEnumerable<Lecturer> expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.Lecturers.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<LecturerRepository>>();
            var repository = new LecturerRepository(_contextFactory, loggerMock.Object);

            // Act
            await using var ctx2 = _contextFactory.CreateDbContext();
            await ctx2.Database.CloseConnectionAsync();
            IEnumerable<Lecturer> filtered = await repository.GetFilteredAsync(default, expressions);

            // Assert
            Assert.That(filtered, Is.EquivalentTo(expected));
        }
        
        [Test]
        public void GetFilteredAsyncTest_ThrowsException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LecturerRepository>>();
            var repository = new LecturerRepository(_contextFactory, loggerMock.Object);
            
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
                var lecturer = new Lecturer { FullName = name, Courses = Array.Empty<Course>() };
                IEnumerable<Lecturer> data = names.Select(s => new Lecturer
                {
                    FullName = s,
                    Courses = Array.Empty<Course>()
                });
                return new object[]
                {
                    data,
                    lecturer,
                    data.Select((std, index) => std with { Id = index + 1}).Append(lecturer with { Id = expectedId })
                };
            }

            yield return GetInsertCase(Enumerable.Empty<string>(), "Test Lecturer", 1);
            yield return GetInsertCase(new[] { "Lecturer 1", "Lecturer 2", "Lecturer 3" }, "Lecturer 4", 4);

            yield return new object[]
            {
                new[] { "Lecturer 1", "Lecturer 2", "Lecturer 3" }.Select(s => new Lecturer
                {
                    FullName = s,
                    Courses = Array.Empty<Course>()
                }),
                new Lecturer { Id = 2, FullName = "Lecturer 2 Changed", Courses = Array.Empty<Course>() },
                new[] { "Lecturer 1", "Lecturer 2 Changed", "Lecturer 3" }.Select((s, i) => new Lecturer
                {
                    Id = i + 1,
                    FullName = s,
                    Courses = Array.Empty<Course>()
                })
            };
        }

        [TestCaseSource(nameof(SaveAsyncTestCaseGenerator))]
        public async Task SaveAsyncTest(IEnumerable<Lecturer> data, Lecturer entity, IEnumerable<Lecturer> expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.Lecturers.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<LecturerRepository>>();
            var repository = new LecturerRepository(_contextFactory, loggerMock.Object);

            // Act
            await repository.SaveAsync(entity);

            // Assert
            await using ApplicationContext ctx2 = _contextFactory.CreateDbContext();
            List<Lecturer> lecturers = await ctx2.Lecturers.Include(lecturer => lecturer.Courses).ToListAsync();
            Assert.That(lecturers, Is.EqualTo(expected));
        }

        [Test]
        public void SaveAsyncTest_ThrowsDataException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LecturerRepository>>();
            var repository = new LecturerRepository(_contextFactory, loggerMock.Object);
            
            // Act
            _contextFactory.SimulateSqlException = true;
            async Task TestAction() => await repository.SaveAsync(new Lecturer { FullName = "Test" });

            // Assert
            Assert.ThrowsAsync<DataException>(TestAction);
        }
        
        [Test]
        public void SaveAsyncTest_ThrowsEntityNotFoundException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LecturerRepository>>();
            var repository = new LecturerRepository(_contextFactory, loggerMock.Object);
            
            // Act
            async Task TestAction() => await repository.SaveAsync(new Lecturer { Id = 1, FullName = "Test" });

            // Assert
            Assert.ThrowsAsync<EntityNotFoundException>(TestAction);
        }
        
        [Test]
        public void SaveAsyncTest_ThrowsIncorrectIdentifierException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LecturerRepository>>();
            var repository = new LecturerRepository(_contextFactory, loggerMock.Object);
            
            // Act
            async Task TestAction() => await repository.SaveAsync(new Lecturer { Id = -1, FullName = "Test" });

            // Assert
            Assert.ThrowsAsync<IncorrectIdentifierException>(TestAction);
        }
        
        #endregion

        #region Delete

        public static IEnumerable<IEnumerable<object>> DeleteAsyncTestCaseGenerator()
        {
            static IEnumerable<object> GetTestCase(IEnumerable<string> names, int id)
            {
                IEnumerable<Lecturer> data = names.Select(s => new Lecturer
                {
                    FullName = s,
                    Courses = Array.Empty<Course>()
                });
                return new object[]
                {
                    data,
                    new Lecturer { Id = id, FullName = "", Courses = Array.Empty<Course>() },
                    data.Select((std, index) => std with { Id = index + 1}).Where(lecturer => lecturer.Id != id)
                };
            }

            yield return GetTestCase(new[] { "Lecturer 1", "Lecturer 2", "Lecturer 3" }, 2);
        }

        [TestCaseSource(nameof(DeleteAsyncTestCaseGenerator))]
        public async Task DeleteAsyncTest(IEnumerable<Lecturer> data, Lecturer entity, IEnumerable<Lecturer> expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.Lecturers.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<LecturerRepository>>();
            var repository = new LecturerRepository(_contextFactory, loggerMock.Object);

            // Act
            await repository.DeleteAsync(entity);

            // Assert
            await using ApplicationContext ctx2 = _contextFactory.CreateDbContext();
            List<Lecturer> lecturers = await ctx2.Lecturers.Include(lecturer => lecturer.Courses).ToListAsync();
            Assert.That(lecturers, Is.EqualTo(expected));
        }

        public static IEnumerable<IEnumerable<object>> DeleteAsyncExceptionTestCaseGenerator()
        {
            static IEnumerable<object> GetTestCase(IEnumerable<string> names, int id)
            {
                IEnumerable<Lecturer> data = names.Select(s => new Lecturer
                {
                    FullName = s,
                    Courses = Array.Empty<Course>()
                });
                return new object[]
                {
                    data,
                    new Lecturer { Id = id, FullName = "", Courses = Array.Empty<Course>() }
                };
            }

            yield return GetTestCase(new[] { "Lecturer 1", "Lecturer 2", "Lecturer 3" }, 5)
                .Append(typeof(EntityNotFoundException))
                .ToArray();
        }

        [TestCaseSource(nameof(DeleteAsyncExceptionTestCaseGenerator))]
        public async Task DeleteAsyncTest_ThrowsException(IEnumerable<Lecturer> data, Lecturer entity, Type expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.Lecturers.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<LecturerRepository>>();
            var repository = new LecturerRepository(_contextFactory, loggerMock.Object);

            // Act

            // Assert
            Assert.ThrowsAsync(expected, async () => await repository.DeleteAsync(entity));
        }
        
        [Test]
        public void DeleteAsyncTest_ThrowsIncorrectIdentifierException([Values(-1, 0)] int id)
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LecturerRepository>>();
            var repository = new LecturerRepository(_contextFactory, loggerMock.Object);
            
            // Act
            async Task TestAction() => await repository.DeleteAsync(new Lecturer { Id = id, FullName = "Test" });

            // Assert
            Assert.ThrowsAsync<IncorrectIdentifierException>(TestAction);
        }
        
        #endregion
    }
}
