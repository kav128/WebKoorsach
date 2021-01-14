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
    public class JournalRecordRepositoryTests
    {
        private SampleDbContextFactory _contextFactory = null!;
        
        [SetUp]
        public void Setup()
        {
            _contextFactory = new SampleDbContextFactory();
            
            using var context = _contextFactory.CreateDbContext();
            context.Lecturers.Add(new Lecturer { FullName = "Test Lecturer" });
            context.SaveChanges();

            context.Courses.Add(new Course { Name = "Test Course", LecturerId = 1 });
            context.SaveChanges();

            context.Lectures.Add(new Lecture { Name = "Test Lecture 1", CourseId = 1 });
            context.Lectures.Add(new Lecture { Name = "Test Lecture 2", CourseId = 1 });
            context.Lectures.Add(new Lecture { Name = "Test Lecture 3", CourseId = 1 });
            context.Lectures.Add(new Lecture { Name = "Test Lecture 4", CourseId = 1 });
            context.Lectures.Add(new Lecture { Name = "Test Lecture 5", CourseId = 1 });
            context.SaveChanges();

            context.Students.Add(new Student { FullName = "Test Student" });
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
            yield return new object?[] { Array.Empty<JournalRecord>(), 1, null };
            
            var journalRecords = new JournalRecord[]
            {
                new() { Score = 0, Attendance = false, LectureId = 1, StudentId = 1 },
                new() { Score = 0, Attendance = false, LectureId = 1, StudentId = 1 },
                new() { Score = 0, Attendance = false, LectureId = 1, StudentId = 1 }
            };
            for (var i = 0; i < journalRecords.Length; i++)
                yield return new object[] { journalRecords, i + 1, journalRecords[i] with { Id = i + 1 } };
        }

        [TestCaseSource(nameof(GetByIdAsyncTestCaseGenerator))]
        public async Task GetByIdAsyncTest(IEnumerable<JournalRecord> data, int id, JournalRecord? expected)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.JournalRecords.AddRangeAsync(data);
                await context.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<JournalRecordRepository>>();
            var repository = new JournalRecordRepository(_contextFactory, loggerMock.Object);
            
            // Act
            JournalRecord? journalRecord = await repository.GetByIdAsync(id);
            
            // Assert
            Assert.That(journalRecord, Is.EqualTo(expected));
        }

        [Test]
        public void GetByIdAsyncTest_ThrowsException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<JournalRecordRepository>>();
            var repository = new JournalRecordRepository(_contextFactory, loggerMock.Object);
            
            // Act
            _contextFactory.SimulateSqlException = true;
            async Task TestAction() => await repository.GetByIdAsync(1);

            // Assert
            Assert.ThrowsAsync<DataException>(TestAction);
        }

        [Test]
        public void GetByIdAsyncTest_ThrowsDataException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<JournalRecordRepository>>();
            var repository = new JournalRecordRepository(_contextFactory, loggerMock.Object);
            
            // Act
            _contextFactory.SimulateSqlException = true;
            async Task TestAction() => await repository.DeleteAsync(new JournalRecord { Id = 1 });

            // Assert
            Assert.ThrowsAsync<DataException>(TestAction);
        }


        #endregion

        #region GetAll

        public static IEnumerable<IEnumerable<object>> GetAllAsyncTestCaseGenerator()
        {
            var journalRecords = new JournalRecord[]
            {
                new() { Score = 0, Attendance = false, LectureId = 1, StudentId = 1 },
                new() { Score = 0, Attendance = false, LectureId = 1, StudentId = 1 },
                new() { Score = 0, Attendance = false, LectureId = 1, StudentId = 1 }
            };

            yield return new[] { journalRecords, journalRecords.Select((journalRecord, index) => journalRecord with { Id = index + 1 }) };
        }

        [TestCaseSource(nameof(GetAllAsyncTestCaseGenerator))]
        public async Task GetAllAsyncTest(IEnumerable<JournalRecord> data, IEnumerable<JournalRecord> expected)
        {
            // Arrange
            await using (ApplicationContext context = _contextFactory.CreateDbContext())
            {
                await context.JournalRecords.AddRangeAsync(data);
                await context.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<JournalRecordRepository>>();
            var repository = new JournalRecordRepository(_contextFactory, loggerMock.Object);

            // Act
            IEnumerable<JournalRecord> journalRecords = await repository.GetAllAsync();
            
            // Assert
            Assert.That(journalRecords, Is.EquivalentTo(expected));
        }
        
        [Test]
        public void GetAllAsyncTest_ThrowsDataException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<JournalRecordRepository>>();
            var repository = new JournalRecordRepository(_contextFactory, loggerMock.Object);
            
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
            var journalRecords = new JournalRecord[]
            {
                new() { Score = 0, Attendance = false, LectureId = 1, StudentId = 1 },
                new() { Score = 0, Attendance = false, LectureId = 1, StudentId = 1 },
                new() { Score = 0, Attendance = false, LectureId = 1, StudentId = 1 }
            };

            yield return new object[] { journalRecords, Array.Empty<Expression<Func<JournalRecord, bool>>>(), journalRecords };
            
            yield return new object[]
            {
                journalRecords,
                new Expression<Func<JournalRecord, bool>>[] { journalRecord => journalRecord.Score > 3 },
                journalRecords.Where(journalRecord => journalRecord.Score > 3)
            };
        }

        [TestCaseSource(nameof(GetFilteredAsyncTestCaseGenerator))]
        public async Task GetFilteredAsyncTest(IEnumerable<JournalRecord> data, Expression<Func<JournalRecord, bool>>[] expressions, IEnumerable<JournalRecord> expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.JournalRecords.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<JournalRecordRepository>>();
            var repository = new JournalRecordRepository(_contextFactory, loggerMock.Object);

            // Act
            await using var ctx2 = _contextFactory.CreateDbContext();
            await ctx2.Database.CloseConnectionAsync();
            IEnumerable<JournalRecord> filtered = await repository.GetFilteredAsync(default, expressions);

            // Assert
            Assert.That(filtered, Is.EquivalentTo(expected));
        }
        
        [Test]
        public void GetFilteredAsyncTest_ThrowsDataException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<JournalRecordRepository>>();
            var repository = new JournalRecordRepository(_contextFactory, loggerMock.Object);
            
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
            static IEnumerable<object> GetInsertCase(IEnumerable<JournalRecord> records, JournalRecord record, int expectedId )
            {
                return new object[]
                {
                    records,
                    record,
                    records.Select((std, index) => std with { Id = index + 1}).Append(record with { Id = expectedId })
                };
            }

            yield return GetInsertCase(Enumerable.Empty<JournalRecord>(),
                new JournalRecord { StudentId = 1, LectureId = 1, Attendance = false, Score = 0 }, 1);
            yield return GetInsertCase(new []{ new JournalRecord { StudentId = 1, LectureId = 1, Attendance = false, Score = 0 } },
                new JournalRecord { StudentId = 1, LectureId = 2, Attendance = false, Score = 0 }, 2);

            yield return new object[]
            {
                new[]
                {
                    new JournalRecord { StudentId = 1, LectureId = 1, Attendance = false, Score = 0 },
                    new JournalRecord { StudentId = 1, LectureId = 2, Attendance = false, Score = 0 },
                    new JournalRecord { StudentId = 1, LectureId = 3, Attendance = true, Score = 3 }
                },
                new JournalRecord { Id = 2, StudentId = 1, LectureId = 2, Attendance = true, Score = 5 },
                new[]
                {
                    new JournalRecord { Id = 1, StudentId = 1, LectureId = 1, Attendance = false, Score = 0 },
                    new JournalRecord { Id = 2, StudentId = 1, LectureId = 2, Attendance = true, Score = 5 },
                    new JournalRecord { Id = 3, StudentId = 1, LectureId = 3, Attendance = true, Score = 3 }
                }
            };
        }

        [TestCaseSource(nameof(SaveAsyncTestCaseGenerator))]
        public async Task SaveAsyncTest(IEnumerable<JournalRecord> data, JournalRecord entity, IEnumerable<JournalRecord> expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.JournalRecords.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<JournalRecordRepository>>();
            var repository = new JournalRecordRepository(_contextFactory, loggerMock.Object);

            // Act
            await repository.SaveAsync(entity);

            // Assert
            await using ApplicationContext ctx2 = _contextFactory.CreateDbContext();
            List<JournalRecord> journalRecords = await ctx2.JournalRecords.ToListAsync();
            Assert.That(journalRecords, Is.EqualTo(expected));
        }

        [Test]
        public void SaveAsyncTest_ThrowsDataException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<JournalRecordRepository>>();
            var repository = new JournalRecordRepository(_contextFactory, loggerMock.Object);
            
            // Act
            _contextFactory.SimulateSqlException = true;
            async Task TestAction() => await repository.SaveAsync(new JournalRecord { LectureId = 1, StudentId = 1, Attendance = false, Score = 0 });

            // Assert
            Assert.ThrowsAsync<DataException>(TestAction);
        }
        
        [Test]
        public void SaveAsyncTest_ThrowsEntityNotFoundException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<JournalRecordRepository>>();
            var repository = new JournalRecordRepository(_contextFactory, loggerMock.Object);
            
            // Act
            async Task TestAction() => await repository.SaveAsync(new JournalRecord { Id = 1, LectureId = 1, StudentId = 1, Attendance = false, Score = 0 });

            // Assert
            Assert.ThrowsAsync<EntityNotFoundException>(TestAction);
        }
        
        [Test]
        public void SaveAsyncTest_ThrowsIncorrectIdentifierException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<JournalRecordRepository>>();
            var repository = new JournalRecordRepository(_contextFactory, loggerMock.Object);
            
            // Act
            async Task TestAction() => await repository.SaveAsync(new JournalRecord { Id = -1, Attendance = true, Score = 5 });

            // Assert
            Assert.ThrowsAsync<IncorrectIdentifierException>(TestAction);
        }
        
        #endregion

        #region Delete

        public static IEnumerable<IEnumerable<object>> DeleteAsyncTestCaseGenerator()
        {
            static IEnumerable<object> GetDeleteCase(IList<JournalRecord> records, int id )
            {
                return new object[]
                {
                    records,
                    records.ElementAt(id - 1),
                    records.Select((std, index) => std with { Id = index + 1}).Where(record => record.Id != id)
                };
            }

            yield return GetDeleteCase(new[] 
            {
                new JournalRecord { StudentId = 1, LectureId = 1, Attendance = false, Score = 0 },
                new JournalRecord { StudentId = 1, LectureId = 2, Attendance = false, Score = 0 },
                new JournalRecord { StudentId = 1, LectureId = 3, Attendance = true, Score = 3 }
            }, 1);
            yield return GetDeleteCase(new[] 
            {
                new JournalRecord { StudentId = 1, LectureId = 1, Attendance = false, Score = 0 },
                new JournalRecord { StudentId = 1, LectureId = 2, Attendance = false, Score = 0 },
                new JournalRecord { StudentId = 1, LectureId = 3, Attendance = true, Score = 3 }
            }, 2);
            yield return GetDeleteCase(new[] 
            {
                new JournalRecord { StudentId = 1, LectureId = 1, Attendance = false, Score = 0 },
                new JournalRecord { StudentId = 1, LectureId = 2, Attendance = false, Score = 0 },
                new JournalRecord { StudentId = 1, LectureId = 3, Attendance = true, Score = 3 }
            }, 3);
        }

        [TestCaseSource(nameof(DeleteAsyncTestCaseGenerator))]
        public async Task DeleteAsyncTest(IEnumerable<JournalRecord> data, JournalRecord entity, IEnumerable<JournalRecord> expected)
        {
            // Arrange
            await using (ApplicationContext ctx1 = _contextFactory.CreateDbContext())
            {
                await ctx1.JournalRecords.AddRangeAsync(data);
                await ctx1.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<JournalRecordRepository>>();
            var repository = new JournalRecordRepository(_contextFactory, loggerMock.Object);

            // Act
            await repository.DeleteAsync(entity);

            // Assert
            await using ApplicationContext ctx2 = _contextFactory.CreateDbContext();
            List<JournalRecord> journalRecords = await ctx2.JournalRecords.ToListAsync();
            Assert.That(journalRecords, Is.EqualTo(expected));
        }
        
        [Test]
        public void DeleteAsyncTest_ThrowsDataException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<JournalRecordRepository>>();
            var repository = new JournalRecordRepository(_contextFactory, loggerMock.Object);

            // Act
            _contextFactory.SimulateSqlException = true;
            async Task TestAction() => await repository.DeleteAsync(new JournalRecord { Id = 1 });

            // Assert
            Assert.ThrowsAsync<DataException>(TestAction);
        }
        
        [Test]
        public void DeleteAsyncTest_ThrowsIncorrectIdentifierException([Values(-1, 0)] int id)
        {
            // Arrange
            var loggerMock = new Mock<ILogger<JournalRecordRepository>>();
            var repository = new JournalRecordRepository(_contextFactory, loggerMock.Object);
            
            // Act
            async Task TestAction() => await repository.DeleteAsync(new JournalRecord { Id = id });

            // Assert
            Assert.ThrowsAsync<IncorrectIdentifierException>(TestAction);
        }
        
                
        [Test]
        public void DeleteAsyncTest_ThrowsEntityNotFoundExceptionException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<JournalRecordRepository>>();
            var repository = new JournalRecordRepository(_contextFactory, loggerMock.Object);
            
            // Act
            async Task TestAction() => await repository.DeleteAsync(new JournalRecord { Id = 1 });

            // Assert
            Assert.ThrowsAsync<EntityNotFoundException>(TestAction);
        }
        
        #endregion
    }
}
