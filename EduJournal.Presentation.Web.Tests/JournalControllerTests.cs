using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EduJournal.BLL.DTO;
using EduJournal.BLL.Services.Entity;
using EduJournal.Presentation.Web.Controllers;
using EduJournal.Presentation.Web.Models;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EduJournal.Presentation.Web.Tests
{
    [TestFixture]
    public class JournalControllerTests
    {
        #region Get

        public static IEnumerable<IEnumerable<object>> GetAllTestCaseGenerator()
        {
            yield return new object[] { 0, 0, 0, Array.Empty<JournalRecordDto>(), Array.Empty<JournalRecordModel>() };
            yield return new object[]
            {
                0, 1, 0,
                new JournalRecordDto[]
                {
                    new() { Attendance = true, Score = 2, LectureId = 1, StudentId = 1 },
                    new() { Attendance = true, Score = 5, LectureId = 2, StudentId = 1 },
                    new() { Attendance = false, Score = 0, LectureId = 3, StudentId = 1 }
                },
                new JournalRecordModel[]
                {
                    new(true, 2, 1, 1),
                    new(true, 5, 1, 2),
                    new(false, 0, 1, 3)
                }
            };
        }

        [TestCaseSource(nameof(GetAllTestCaseGenerator))]
        public async Task GetAllTest(int lectureId,
            int studentId,
            int courseId,
            IList<JournalRecordDto> data,
            IList<JournalRecordModel> expected)
        {
            // Arrange
            Mock<IJournalService> serviceMock = new();
            serviceMock.Setup(service => service.GetRecords(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns<int, int, int>(
                (i, j, k) =>
                    i == lectureId && j == studentId && k == courseId
                        ? Task.FromResult(data)
                        : Task.FromResult(Array.Empty<JournalRecordDto>() as IList<JournalRecordDto>));
            Mock<IMapper> mapperMock = new();
            mapperMock.Setup(mapper => mapper.Map<IList<JournalRecordModel>>(data)).Returns(expected).Verifiable();
            var loggerMock = new Mock<ILogger<JournalService>>();
            var controller = new JournalController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

            // Act
            IList<JournalRecordModel> models = await controller.GetAll(lectureId, studentId, courseId);

            // Assert
            Assert.That(models, Is.EquivalentTo(expected));
            mapperMock.Verify();
            serviceMock.Verify(service => service.GetRecords(lectureId, studentId, courseId));
            serviceMock.VerifyNoOtherCalls();
        }

        #endregion

        #region Post

        public static IEnumerable<IEnumerable<object>> InsertTestCaseGenerator()
        {
            yield return new object[]
            {
                new JournalRecordModel(true, 5, 3, 1),
                new JournalRecordDto { Attendance = true, Score = 5, StudentId = 3, LectureId = 1 }
            };
        }

        [TestCaseSource(nameof(InsertTestCaseGenerator))]
        public async Task InsertTest(JournalRecordModel model, JournalRecordDto expected)
        {
            // Arrange
            Mock<IMapper> mapperMock = new();
            mapperMock.Setup(mapper => mapper.Map<JournalRecordDto>(model)).Returns(expected).Verifiable();
            Mock<IJournalService> serviceMock = new();
            serviceMock.Setup(service => service.SaveRecord(expected)).Verifiable();
            var loggerMock = new Mock<ILogger<JournalService>>();
            var controller = new JournalController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

            // Act
            await controller.Insert(model);

            // Assert
            mapperMock.Verify();
            serviceMock.Verify();
            serviceMock.VerifyNoOtherCalls();
        }

        #endregion
    }
}
