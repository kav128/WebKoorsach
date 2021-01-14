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
    public class StudentRepositoryTests
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
            yield return new object?[] { Enumerable.Empty<Student>(), 1, null };
            
            var students = new Student[]
            {
                new() { FullName = "Student 1", JournalRecords = Enumerable.Empty<JournalRecord>() },
                new() { FullName = "Student 2", JournalRecords = Enumerable.Empty<JournalRecord>() },
                new() { FullName = "Student 3", JournalRecords = Enumerable.Empty<JournalRecord>() }
            };
            for (var i = 0; i < students.Length; i++)
                yield return new object[] { students, i + 1, students[i] with { Id = i + 1 } };
        }

        [TestCaseSource(nameof(GetByIdAsyncTestCaseGenerator))]
        public async Task GetByIdAsyncTest(IEnumerable<Student> data, int id, Student? expected)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Students.AddRangeAsync(data);
                await context.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<StudentRepository>>();
            var repository = new StudentRepository(_contextFactory, loggerMock.Object);
            
            // Act
            Student? student = await repository.GetByIdAsync(id);
            
            // Assert
            Assert.That(student, Is.EqualTo(expected));
        }

        [Test]
        public void GetByIdAsyncTest_ThrowsException([Values(1)] int id)
        {
            // Arrange
            var loggerMock = new Mock<ILogger<StudentRepository>>();
            var repository = new StudentRepository(_contextFactory, loggerMock.Object);
            
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
            var loggerMock = new Mock<ILogger<StudentRepository>>();
            var repository = new StudentRepository(_contextFactory, loggerMock.Object);
            
            // Act
            _contextFactory.SimulateSqlException = true;
            async Task TestAction() => await repository.DeleteAsync(new Student { Id = 1 });

            // Assert
            Assert.ThrowsAsync<DataException>(TestAction);
        }

        #endregion

        #region GetAll

        public static IEnumerable<IEnumerable<object>> GetAllAsyncTestCaseGenerator()
        {
            var students = new Student[]
            {
                new() { FullName = "Student 1", JournalRecords = Enumerable.Empty<JournalRecord>() },
                new() { FullName = "Student 2", JournalRecords = Enumerable.Empty<JournalRecord>() },
                new() { FullName = "Student 3", JournalRecords = Enumerable.Empty<JournalRecord>() }
            };

            yield return new[] { students, students.Select((student, index) => student with { Id = index + 1 }) };
        }

        [TestCaseSource(nameof(GetAllAsyncTestCaseGenerator))]
        public async Task GetAllAsyncTest(IEnumerable<Student> data, IEnumerable<Student> expected)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.Students.AddRangeAsync(data);
                await context.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<StudentRepository>>();
            var repository = new StudentRepository(_contextFactory, loggerMock.Object);

            // Act
            IEnumerable<Student> students = await repository.GetAllAsync();
            
            // Assert
            Assert.That(students, Is.EquivalentTo(expected));
        }
        
        [Test]
        public void GetAllAsyncTest_ThrowsException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<StudentRepository>>();
            var repository = new StudentRepository(_contextFactory, loggerMock.Object);
            
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
            var students = new Student[]
            {
                new() { FullName = "Student 1", JournalRecords = Enumerable.Empty<JournalRecord>() },
                new() { FullName = "Student 2", JournalRecords = Enumerable.Empty<JournalRecord>() },
                new() { FullName = "Student 3", JournalRecords = Enumerable.Empty<JournalRecord>() }
            };

            yield return new object[] { students, Array.Empty<Expression<Func<Student, bool>>>(), students };
            
            yield return new object[]
            {
                students,
                new Expression<Func<Student, bool>>[] { student => student.FullName == "Student 2" },
                students.Where(student => student.FullName == "Student 2")
            };
        }

        [TestCaseSource(nameof(GetFilteredAsyncTestCaseGenerator))]
        public async Task GetFilteredAsyncTest(IEnumerable<Student> data, Expression<Func<Student, bool>>[] expressions, IEnumerable<Student> expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.Students.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<StudentRepository>>();
            var repository = new StudentRepository(_contextFactory, loggerMock.Object);

            // Act
            await using var ctx2 = _contextFactory.CreateDbContext();
            await ctx2.Database.CloseConnectionAsync();
            IEnumerable<Student> filtered = await repository.GetFilteredAsync(default, expressions);

            // Assert
            Assert.That(filtered, Is.EquivalentTo(expected));
        }
        
        [Test]
        public void GetFilteredAsyncTest_ThrowsException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<StudentRepository>>();
            var repository = new StudentRepository(_contextFactory, loggerMock.Object);
            
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
                var student = new Student { FullName = name, JournalRecords = Enumerable.Empty<JournalRecord>() };
                IEnumerable<Student> data = names.Select(s => new Student
                {
                    FullName = s,
                    JournalRecords = Enumerable.Empty<JournalRecord>()
                });
                return new object[]
                {
                    data,
                    student,
                    data.Select((std, index) => std with { Id = index + 1}).Append(student with { Id = expectedId })
                };
            }

            yield return GetInsertCase(Enumerable.Empty<string>(), "Test Student", 1);
            yield return GetInsertCase(new[] { "Student 1", "Student 2", "Student 3" }, "Student 4", 4);

            yield return new object[]
            {
                new[] { "Student 1", "Student 2", "Student 3" }.Select(s => new Student
                {
                    FullName = s,
                    JournalRecords = Enumerable.Empty<JournalRecord>()
                }),
                new Student { Id = 2, FullName = "Student 2 Changed", JournalRecords = Enumerable.Empty<JournalRecord>() },
                new[] { "Student 1", "Student 2 Changed", "Student 3" }.Select((s, i) => new Student
                {
                    Id = i + 1,
                    FullName = s,
                    JournalRecords = Enumerable.Empty<JournalRecord>()
                })
            };
        }

        [TestCaseSource(nameof(SaveAsyncTestCaseGenerator))]
        public async Task SaveAsyncTest(IEnumerable<Student> data, Student entity, IEnumerable<Student> expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.Students.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<StudentRepository>>();
            var repository = new StudentRepository(_contextFactory, loggerMock.Object);

            // Act
            await repository.SaveAsync(entity);

            // Assert
            await using ApplicationContext ctx2 = _contextFactory.CreateDbContext();
            List<Student> students = await ctx2.Students.Include(student => student.JournalRecords).ToListAsync();
            Assert.That(students, Is.EqualTo(expected));
        }

        [Test]
        public void SaveAsyncTest_ThrowsDataException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<StudentRepository>>();
            var repository = new StudentRepository(_contextFactory, loggerMock.Object);
            
            // Act
            _contextFactory.SimulateSqlException = true;
            async Task TestAction() => await repository.SaveAsync(new Student { FullName = "Test" });

            // Assert
            Assert.ThrowsAsync<DataException>(TestAction);
        }
        
        [Test]
        public void SaveAsyncTest_ThrowsEntityNotFoundException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<StudentRepository>>();
            var repository = new StudentRepository(_contextFactory, loggerMock.Object);
            
            // Act
            async Task TestAction() => await repository.SaveAsync(new Student { Id = 1, FullName = "Test" });

            // Assert
            Assert.ThrowsAsync<EntityNotFoundException>(TestAction);
        }
        
        [Test]
        public void SaveAsyncTest_ThrowsIncorrectIdentifierException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<StudentRepository>>();
            var repository = new StudentRepository(_contextFactory, loggerMock.Object);
            
            // Act
            async Task TestAction() => await repository.SaveAsync(new Student { Id = -1, FullName = "Test" });

            // Assert
            Assert.ThrowsAsync<IncorrectIdentifierException>(TestAction);
        }
        
        #endregion

        #region Delete

        public static IEnumerable<IEnumerable<object>> DeleteAsyncTestCaseGenerator()
        {
            static IEnumerable<object> GetTestCase(IEnumerable<string> names, int id)
            {
                IEnumerable<Student> data = names.Select(s => new Student
                {
                    FullName = s,
                    JournalRecords = Enumerable.Empty<JournalRecord>()
                });
                return new object[]
                {
                    data,
                    new Student { Id = id, FullName = "", JournalRecords = Enumerable.Empty<JournalRecord>() },
                    data.Select((std, index) => std with { Id = index + 1}).Where(student => student.Id != id)
                };
            }

            yield return GetTestCase(new[] { "Student 1", "Student 2", "Student 3" }, 2);
        }

        [TestCaseSource(nameof(DeleteAsyncTestCaseGenerator))]
        public async Task DeleteAsyncTest(IEnumerable<Student> data, Student entity, IEnumerable<Student> expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.Students.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<StudentRepository>>();
            var repository = new StudentRepository(_contextFactory, loggerMock.Object);

            // Act
            await repository.DeleteAsync(entity);

            // Assert
            await using ApplicationContext ctx2 = _contextFactory.CreateDbContext();
            List<Student> students = await ctx2.Students.Include(student => student.JournalRecords).ToListAsync();
            Assert.That(students, Is.EqualTo(expected));
        }

        public static IEnumerable<IEnumerable<object>> DeleteAsyncExceptionTestCaseGenerator()
        {
            static IEnumerable<object> GetTestCase(IEnumerable<string> names, int id)
            {
                IEnumerable<Student> data = names.Select(s => new Student
                {
                    FullName = s,
                    JournalRecords = Enumerable.Empty<JournalRecord>()
                });
                return new object[]
                {
                    data,
                    new Student { Id = id, FullName = "", JournalRecords = Enumerable.Empty<JournalRecord>() }
                };
            }

            yield return GetTestCase(new[] { "Student 1", "Student 2", "Student 3" }, 5)
                .Append(typeof(EntityNotFoundException))
                .ToArray();
        }

        [TestCaseSource(nameof(DeleteAsyncExceptionTestCaseGenerator))]
        public async Task DeleteAsyncTest_ThrowsException(IEnumerable<Student> data, Student entity, Type expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.Students.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<StudentRepository>>();
            var repository = new StudentRepository(_contextFactory, loggerMock.Object);

            // Act

            // Assert
            Assert.ThrowsAsync(expected, async () => await repository.DeleteAsync(entity));
        }
        
        [Test]
        public void DeleteAsyncTest_ThrowsIncorrectIdentifierException([Values(-1, 0)] int id)
        {
            // Arrange
            var loggerMock = new Mock<ILogger<StudentRepository>>();
            var repository = new StudentRepository(_contextFactory, loggerMock.Object);
            
            // Act
            async Task TestAction() => await repository.DeleteAsync(new Student { Id = id, FullName = "Test" });

            // Assert
            Assert.ThrowsAsync<IncorrectIdentifierException>(TestAction);
        }

        #endregion
    }
}
