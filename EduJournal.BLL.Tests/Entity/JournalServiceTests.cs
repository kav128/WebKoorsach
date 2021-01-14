using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EduJournal.BLL.DTO;
using EduJournal.BLL.Services.Entity;
using EduJournal.BLL.Services.Report;
using EduJournal.DAL.Entities;
using EduJournal.DAL.Repositories.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EduJournal.BLL.Tests.Entity
{
    [TestFixture]
    public class JournalServiceTests
    {
        private static IEnumerable<IEnumerable<object>> SaveRecordTestCaseGenerator()
        {
            yield return new object[]
            {
                new JournalRecordDto { Attendance = false, Score = 0, LectureId = 1, StudentId = 1 }, 0,
                new LectureDto { Id = 1, Name = "Test", CourseId = 1 },
                new StudentDto { Id = 1, FullName = "Test" },
                new CourseDto { Id = 1, Name = "Test", LectureIds = new[] { 1 }, LecturerId = 1 },
                new JournalRecord { Id = default, Attendance = false, Score = 0, LectureId = 1, StudentId = 1 }
            };
            
            yield return new object[]
            {
                new JournalRecordDto { Attendance = false, Score = 0, LectureId = 1, StudentId = 1 }, 5,
                new LectureDto { Id = 1, Name = "Test", CourseId = 1 },
                new StudentDto { Id = 1, FullName = "Test" },
                new CourseDto { Id = 1, Name = "Test", LectureIds = new[] { 1 }, LecturerId = 1 },
                new JournalRecord { Id = 5, Attendance = false, Score = 0, LectureId = 1, StudentId = 1 }
            };
        }

        [TestCaseSource(nameof(SaveRecordTestCaseGenerator))]
        public async Task SaveRecordTest(JournalRecordDto dto, int repositoryId, LectureDto lectureDto, StudentDto studentDto, CourseDto courseDto, JournalRecord expected)
        {
            // Arrange
            var repositoryMock = new Mock<ICrudRepository<JournalRecord>>();
            repositoryMock.Setup(repository => repository.GetFilteredAsync(It.IsAny<CancellationToken>(),
                    It.IsAny<Expression<Func<JournalRecord, bool>>[]>()))
                .ReturnsAsync(new[] { new JournalRecord { Id = repositoryId } });
            var messageSenderMock = new Mock<IMessageSender>();
            var messageSenderFactoryMock = new Mock<IMessageSenderFactory>();
            messageSenderFactoryMock.Setup(factory => factory.GetEmailSender()).Returns(messageSenderMock.Object);
            messageSenderFactoryMock.Setup(factory => factory.GetSmsSender()).Returns(messageSenderMock.Object);
            var courseServiceMock = new Mock<ICourseService>();
            courseServiceMock.Setup(courseService => courseService.GetCourse(courseDto.Id)).ReturnsAsync(courseDto);
            var lectureServiceMock = new Mock<ILectureService>();
            lectureServiceMock.Setup(lectureService => lectureService.GetLecture(dto.LectureId)).ReturnsAsync(lectureDto);
            var studentServiceMock = new Mock<IStudentService>();
            studentServiceMock.Setup(studentService => studentService.GetStudent(dto.StudentId)).ReturnsAsync(studentDto);
            var lecturerServiceMock = new Mock<ILecturerService>();
            var loggerMock = new Mock<ILogger<JournalService>>();
            JournalService service = new(repositoryMock.Object,
                messageSenderFactoryMock.Object,
                courseServiceMock.Object,
                lectureServiceMock.Object,
                studentServiceMock.Object,
                lecturerServiceMock.Object,
                loggerMock.Object);

            // Act
            await service.SaveRecord(dto);
            
            // Assert
            repositoryMock.Verify(repository => repository.GetFilteredAsync(It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<JournalRecord, bool>>[]>()));
            repositoryMock.Verify(repository => repository.SaveAsync(expected,
                    It.IsAny<CancellationToken>()), Times.Once());
            repositoryMock.VerifyNoOtherCalls();
        }

        private static IEnumerable<IEnumerable<object>> GetRecordsTestCaseGenerator()
        {
            var data = new[]
            {
                new JournalRecord { Id = 1, Attendance = false, Score = 0, LectureId = 1, Lecture = new Lecture { Id = 1, Name = "Test Lecture 1", CourseId = 1 }, StudentId = 1 },
                new JournalRecord { Id = 2, Attendance = true, Score = 5, LectureId = 3, Lecture = new Lecture { Id = 3, Name = "Test Lecture 3", CourseId = 3 }, StudentId = 4 },
                new JournalRecord { Id = 3, Attendance = true, Score = 2, LectureId = 4, Lecture = new Lecture { Id = 4, Name = "Test Lecture 4", CourseId = 4 }, StudentId = 2 },
                new JournalRecord { Id = 4, Attendance = false, Score = 0, LectureId = 1, Lecture = new Lecture { Id = 1, Name = "Test Lecture 1", CourseId = 1 }, StudentId = 2 }
            };
            var recordDtos = new[]
            {
                new JournalRecordDto { Attendance = false, Score = 0, LectureId = 1, StudentId = 1 },
                new JournalRecordDto { Attendance = true, Score = 5, LectureId = 3, StudentId = 4},
                new JournalRecordDto { Attendance = true, Score = 2, LectureId = 4, StudentId = 2},
                new JournalRecordDto { Attendance = false, Score = 0, LectureId = 1, StudentId = 2}
            };
            
            yield return new object[]
            {
                default(int), default(int), default(int),
                data,
                recordDtos
            };
            yield return new object[]
            {
                1, default(int), default(int),
                data.Where(record => record.LectureId == 1),
                recordDtos.Where(dto => dto.LectureId == 1)
            };
            yield return new object[]
            {
                default(int), 4, default(int),
                data.Where(record => record.StudentId == 4),
                recordDtos.Where(dto => dto.StudentId == 4)
            };
            yield return new object[]
            {
                3, 4, default(int),
                data.Where(record => record is { LectureId: 3, StudentId: 4 }),
                recordDtos.Where(dto => dto is { LectureId: 3, StudentId: 4 })
            };
            yield return new object[]
            {
                default(int), 2, 4,
                data.Where(record => record is { Lecture: { CourseId: 4 }, StudentId: 2 }),
                recordDtos.Where(dto => dto is { LectureId: 4, StudentId: 2 })
            };
        }

        [TestCaseSource(nameof(GetRecordsTestCaseGenerator))]
        public async Task GetRecordsTest(int lectureId,
            int studentId,
            int courseId,
            IEnumerable<JournalRecord> records,
            IEnumerable<JournalRecordDto> expected)
        {
            // Arrange
            Mock<ICrudRepository<JournalRecord>> journalRepositoryMock = new();
            journalRepositoryMock.Setup(repository => repository.GetFilteredAsync(It.IsAny<CancellationToken>(),
                    It.IsAny<Expression<Func<JournalRecord, bool>>[]>()))
                .Returns(Task.FromResult(records))
                .Verifiable();
            var messageSenderFactoryMock = new Mock<IMessageSenderFactory>();
            var courseServiceMock = new Mock<ICourseService>();
            var lectureServiceMock = new Mock<ILectureService>();
            var studentServiceMock = new Mock<IStudentService>();
            var lecturerServiceMock = new Mock<ILecturerService>();
            var loggerMock = new Mock<ILogger<JournalService>>();
            var service = new JournalService(journalRepositoryMock.Object,
                messageSenderFactoryMock.Object,
                courseServiceMock.Object,
                lectureServiceMock.Object,
                studentServiceMock.Object,
                lecturerServiceMock.Object,
                loggerMock.Object);

            // Act
            IList<JournalRecordDto> dtos = await service.GetRecords(lectureId, studentId);

            // Assert
            Assert.That(dtos, Is.EquivalentTo(expected));
            journalRepositoryMock.Verify();
            journalRepositoryMock.VerifyNoOtherCalls();
        }
    }
}
