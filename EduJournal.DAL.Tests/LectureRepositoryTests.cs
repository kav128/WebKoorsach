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
    public class LectureRepositoryTests
    {
        private SampleDbContextFactory _contextFactory = null!;
        
        [SetUp]
        public void Setup()
        {
            _contextFactory = new SampleDbContextFactory();
            using ApplicationContext context = _contextFactory.CreateDbContext();
            context.Lecturers.Add(new Lecturer { FullName = "Test Lecturer" });
            context.SaveChanges();
            context.Courses.Add(new Course { Name = "Test Course", LecturerId = 1});
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
            yield return new object?[] { Array.Empty<Lecture>(), 1, null };
            
            var lectures = new Lecture[]
            {
                new() { Name = "Lecture 1", JournalRecords = Enumerable.Empty<JournalRecord>(), CourseId = 1 },
                new() { Name = "Lecture 2", JournalRecords = Enumerable.Empty<JournalRecord>(), CourseId = 1 },
                new() { Name = "Lecture 3", JournalRecords = Enumerable.Empty<JournalRecord>(), CourseId = 1 }
            };
            for (var i = 0; i < lectures.Length; i++)
                yield return new object[] { lectures, i + 1, lectures[i] with { Id = i + 1 } };
        }

        [TestCaseSource(nameof(GetByIdAsyncTestCaseGenerator))]
        public async Task GetByIdAsyncTest(IEnumerable<Lecture> data, int id, Lecture? expected)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lectures.AddRangeAsync(data);
                await context.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<LectureRepository>>();
            var repository = new LectureRepository(_contextFactory, loggerMock.Object);
            
            // Act
            Lecture? lecture = await repository.GetByIdAsync(id);
            
            // Assert
            Assert.That(lecture, Is.EqualTo(expected));
        }

        [Test]
        public void GetByIdAsyncTest_ThrowsException([Values(1)] int id)
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LectureRepository>>();
            var repository = new LectureRepository(_contextFactory, loggerMock.Object);
            
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
            var loggerMock = new Mock<ILogger<LectureRepository>>();
            var repository = new LectureRepository(_contextFactory, loggerMock.Object);
            
            // Act
            _contextFactory.SimulateSqlException = true;
            async Task TestAction() => await repository.DeleteAsync(new Lecture { Id = 1 });

            // Assert
            Assert.ThrowsAsync<DataException>(TestAction);
        }


        #endregion

        #region GetAll

        public static IEnumerable<IEnumerable<object>> GetAllAsyncTestCaseGenerator()
        {
            var lectures = new Lecture[]
            {
                new() { Name = "Lecture 1", JournalRecords = Enumerable.Empty<JournalRecord>(), CourseId = 1 },
                new() { Name = "Lecture 2", JournalRecords = Enumerable.Empty<JournalRecord>(), CourseId = 1 },
                new() { Name = "Lecture 3", JournalRecords = Enumerable.Empty<JournalRecord>(), CourseId = 1 }
            };

            yield return new[] { lectures, lectures.Select((lecture, index) => lecture with { Id = index + 1 }) };
        }

        [TestCaseSource(nameof(GetAllAsyncTestCaseGenerator))]
        public async Task GetAllAsyncTest(IEnumerable<Lecture> data, IEnumerable<Lecture> expected)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Lectures.AddRangeAsync(data);
                await context.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<LectureRepository>>();
            var repository = new LectureRepository(_contextFactory, loggerMock.Object);

            // Act
            IEnumerable<Lecture> lectures = await repository.GetAllAsync();
            
            // Assert
            Assert.That(lectures, Is.EquivalentTo(expected));
        }
        
        [Test]
        public void GetAllAsyncTest_ThrowsException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LectureRepository>>();
            var repository = new LectureRepository(_contextFactory, loggerMock.Object);
            
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
            var lectures = new Lecture[]
            {
                new() { Name = "Lecture 1", JournalRecords = Enumerable.Empty<JournalRecord>(), CourseId = 1 },
                new() { Name = "Lecture 2", JournalRecords = Enumerable.Empty<JournalRecord>(), CourseId = 1 },
                new() { Name = "Lecture 3", JournalRecords = Enumerable.Empty<JournalRecord>(), CourseId = 1 }
            };

            yield return new object[] { lectures, Array.Empty<Expression<Func<Lecture, bool>>>(), lectures };
            
            yield return new object[]
            {
                lectures,
                new Expression<Func<Lecture, bool>>[] { lecture => lecture.Name == "Lecture 2" },
                lectures.Where(lecture => lecture.Name == "Lecture 2")
            };
        }

        [TestCaseSource(nameof(GetFilteredAsyncTestCaseGenerator))]
        public async Task GetFilteredAsyncTest(IEnumerable<Lecture> data, Expression<Func<Lecture, bool>>[] expressions, IEnumerable<Lecture> expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.Lectures.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<LectureRepository>>();
            var repository = new LectureRepository(_contextFactory, loggerMock.Object);

            // Act
            await using var ctx2 = _contextFactory.CreateDbContext();
            await ctx2.Database.CloseConnectionAsync();
            IEnumerable<Lecture> filtered = await repository.GetFilteredAsync(default, expressions);

            // Assert
            Assert.That(filtered, Is.EquivalentTo(expected));
        }
        
        [Test]
        public void GetFilteredAsyncTest_ThrowsException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LectureRepository>>();
            var repository = new LectureRepository(_contextFactory, loggerMock.Object);
            
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
                var lecture = new Lecture { Name = name, JournalRecords = Enumerable.Empty<JournalRecord>(), CourseId = 1 };
                IEnumerable<Lecture> data = names.Select(s => new Lecture
                {
                    Name = s,
                    JournalRecords = Enumerable.Empty<JournalRecord>(),
                    CourseId = 1
                });
                return new object[]
                {
                    data,
                    lecture,
                    data.Select((std, index) => std with { Id = index + 1}).Append(lecture with { Id = expectedId })
                };
            }

            yield return GetInsertCase(Enumerable.Empty<string>(), "Test Lecture", 1);
            yield return GetInsertCase(new[] { "Lecture 1", "Lecture 2", "Lecture 3" }, "Lecture 4", 4);

            yield return new object[]
            {
                new[] { "Lecture 1", "Lecture 2", "Lecture 3" }.Select(s => new Lecture
                {
                    Name = s,
                    JournalRecords = Enumerable.Empty<JournalRecord>(),
                    CourseId = 1
                }),
                new Lecture { Id = 2, Name = "Lecture 2 Changed", JournalRecords = Enumerable.Empty<JournalRecord>(), CourseId = 1 },
                new[] { "Lecture 1", "Lecture 2 Changed", "Lecture 3" }.Select((s, i) => new Lecture
                {
                    Id = i + 1,
                    Name = s,
                    JournalRecords = Enumerable.Empty<JournalRecord>(),
                    CourseId = 1
                })
            };
        }

        [TestCaseSource(nameof(SaveAsyncTestCaseGenerator))]
        public async Task SaveAsyncTest(IEnumerable<Lecture> data, Lecture entity, IEnumerable<Lecture> expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.Lectures.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<LectureRepository>>();
            var repository = new LectureRepository(_contextFactory, loggerMock.Object);

            // Act
            await repository.SaveAsync(entity);

            // Assert
            await using ApplicationContext ctx2 = _contextFactory.CreateDbContext();
            List<Lecture> lectures = await ctx2.Lectures.Include(lecture => lecture.JournalRecords).ToListAsync();
            Assert.That(lectures, Is.EqualTo(expected));
        }

        [Test]
        public void SaveAsyncTest_ThrowsDataException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LectureRepository>>();
            var repository = new LectureRepository(_contextFactory, loggerMock.Object);
            
            // Act
            _contextFactory.SimulateSqlException = true;
            async Task TestAction() => await repository.SaveAsync(new Lecture { Name = "Test" });

            // Assert
            Assert.ThrowsAsync<DataException>(TestAction);
        }
        
        [Test]
        public void SaveAsyncTest_ThrowsEntityNotFoundException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LectureRepository>>();
            var repository = new LectureRepository(_contextFactory, loggerMock.Object);
            
            // Act
            async Task TestAction() => await repository.SaveAsync(new Lecture { Id = 1, Name = "Test" });

            // Assert
            Assert.ThrowsAsync<EntityNotFoundException>(TestAction);
        }
        
        [Test]
        public void SaveAsyncTest_ThrowsIncorrectIdentifierException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LectureRepository>>();
            var repository = new LectureRepository(_contextFactory, loggerMock.Object);
            
            // Act
            async Task TestAction() => await repository.SaveAsync(new Lecture { Id = -1, Name = "Test" });

            // Assert
            Assert.ThrowsAsync<IncorrectIdentifierException>(TestAction);
        }
        
        #endregion

        #region Delete

        public static IEnumerable<IEnumerable<object>> DeleteAsyncTestCaseGenerator()
        {
            static IEnumerable<object> GetTestCase(IEnumerable<string> names, int id)
            {
                IEnumerable<Lecture> data = names.Select(s => new Lecture
                {
                    Name = s,
                    JournalRecords = Enumerable.Empty<JournalRecord>(),
                    CourseId = 1
                });
                return new object[]
                {
                    data,
                    new Lecture { Id = id, Name = "", JournalRecords = Enumerable.Empty<JournalRecord>(), CourseId = 1 },
                    data.Select((std, index) => std with { Id = index + 1}).Where(lecture => lecture.Id != id)
                };
            }

            yield return GetTestCase(new[] { "Lecture 1", "Lecture 2", "Lecture 3" }, 2);
        }

        [TestCaseSource(nameof(DeleteAsyncTestCaseGenerator))]
        public async Task DeleteAsyncTest(IEnumerable<Lecture> data, Lecture entity, IEnumerable<Lecture> expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.Lectures.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<LectureRepository>>();
            var repository = new LectureRepository(_contextFactory, loggerMock.Object);

            // Act
            await repository.DeleteAsync(entity);

            // Assert
            await using ApplicationContext ctx2 = _contextFactory.CreateDbContext();
            List<Lecture> lectures = await ctx2.Lectures.Include(lecture => lecture.JournalRecords).ToListAsync();
            Assert.That(lectures, Is.EqualTo(expected));
        }

        public static IEnumerable<IEnumerable<object>> DeleteAsyncExceptionTestCaseGenerator()
        {
            static IEnumerable<object> GetTestCase(IEnumerable<string> names, int id)
            {
                IEnumerable<Lecture> data = names.Select(s => new Lecture
                {
                    Name = s,
                    JournalRecords = Enumerable.Empty<JournalRecord>(),
                    CourseId = 1
                });
                return new object[]
                {
                    data,
                    new Lecture { Id = id, Name = "", JournalRecords = Enumerable.Empty<JournalRecord>(), CourseId = 1 }
                };
            }

            yield return GetTestCase(new[] { "Lecture 1", "Lecture 2", "Lecture 3" }, 5)
                .Append(typeof(EntityNotFoundException))
                .ToArray();
        }

        [TestCaseSource(nameof(DeleteAsyncExceptionTestCaseGenerator))]
        public async Task DeleteAsyncTest_ThrowsException(IEnumerable<Lecture> data, Lecture entity, Type expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.Lectures.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<LectureRepository>>();
            var repository = new LectureRepository(_contextFactory, loggerMock.Object);

            // Act

            // Assert
            Assert.ThrowsAsync(expected, async () => await repository.DeleteAsync(entity));
        }
        
        [Test]
        public void DeleteAsyncTest_ThrowsIncorrectIdentifierException([Values(-1, 0)] int id)
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LectureRepository>>();
            var repository = new LectureRepository(_contextFactory, loggerMock.Object);
            
            // Act
            async Task TestAction() => await repository.DeleteAsync(new Lecture { Id = id, Name = "Test" });

            // Assert
            Assert.ThrowsAsync<IncorrectIdentifierException>(TestAction);
        }
        
        #endregion
    }
}
