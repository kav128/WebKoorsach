using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EduJournal.BLL.DTO;
using EduJournal.BLL.Services.Entity;
using EduJournal.Presentation.Web.Controllers;
using EduJournal.Presentation.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EduJournal.Presentation.Web.Tests
{
    [TestFixture]
    public class LectureControllerTests
    {
        #region GetAll

        public static IEnumerable<IEnumerable<object>> GetAllTestCaseGenerator()
        {
            yield return new object[]
            {
                new LectureDto[]
                {
                    new() { Id = 1, Name = "Lecture 1", CourseId = 1 }
                },
                new LectureModel[] { new(1, "Lecture 1", 1) }
            };
            yield return new object[]
            {
                new LectureDto[]
                {
                    new() { Id = 1, Name = "Lecture 1", CourseId = 1 },
                    new() { Id = 2, Name = "Lecture 2", CourseId = 2 }
                },
                new LectureModel[]
                {
                    new(1, "Lecture 1", 1),
                    new(2, "Lecture 2", 2)
                }
            };
        }

        [TestCaseSource(nameof(GetAllTestCaseGenerator))]
        public async Task GetAllTest(IList<LectureDto> data, IList<LectureModel> expected)
        {
            // Arrange
            Mock<ILectureService> serviceMock = new();
            serviceMock.Setup(service => service.GetAll()).ReturnsAsync(() => data).Verifiable();
            Mock<IMapper> mapperMock = new();
            mapperMock.Setup(mapper => mapper.Map<IList<LectureModel>>(data)).Returns(expected).Verifiable();
            var loggerMock = new Mock<ILogger<LectureController>>();
            var controller = new LectureController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

            // Act
            ActionResult<IList<LectureModel>> actionResult = await controller.GetAll();
            IList<LectureModel> models = actionResult.Value;

            // Assert
            Assert.That(models, Is.EquivalentTo(expected));
            mapperMock.Verify();
            serviceMock.Verify();
            serviceMock.VerifyNoOtherCalls();
        }

        #endregion

        #region GetById

        public static IEnumerable<IEnumerable<object>> GetByIdTestCaseGenerator()
        {
            yield return new object[]
            {
                new LectureDto[]
                {
                    new() { Id = 1, Name = "Lecture 1", CourseId = 1 }
                },
                new LectureDto { Id = 1, Name = "Lecture 1", CourseId = 1 },
                1,
                new LectureModel(1, "Lecture 1", 1)
            };
            yield return new object[]
            {
                new LectureDto[]
                {
                    new() { Id = 1, Name = "Lecture 1", CourseId = 1 },
                    new() { Id = 2, Name = "Lecture 2", CourseId = 1 }
                },
                new LectureDto { Id = 2, Name = "Lecture 2", CourseId = 1 },
                2,
                new LectureModel(2, "Lecture 2", 1)
            };
            yield return new object[]
            {
                new LectureDto[]
                {
                    new() { Id = 1, Name = "Lecture 1", CourseId = 1 },
                    new() { Id = 2, Name = "Lecture 2", CourseId = 1 }
                },
                null,
                3,
                null
            };
        }

        [TestCaseSource(nameof(GetByIdTestCaseGenerator))]
        public async Task GetByIdTest(IList<LectureDto> data, LectureDto dto, int id, LectureModel expected)
        {
            // Arrange
            Mock<ILectureService> serviceMock = new();
            serviceMock.Setup(service => service.GetLecture(id)).ReturnsAsync(() => dto).Verifiable();
            Mock<IMapper> mapperMock = new();
            mapperMock.Setup(mapper => mapper.Map<LectureModel>(dto)).Returns(expected);
            var loggerMock = new Mock<ILogger<LectureController>>();
            var controller = new LectureController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

            // Act
            ActionResult<LectureModel> actionResult = await controller.GetById(id);
            LectureModel model = actionResult.Value;

            // Assert
            Assert.That(model, Is.EqualTo(expected));
            serviceMock.Verify();
            serviceMock.VerifyNoOtherCalls();
            if (dto is not null) mapperMock.Verify(mapper => mapper.Map<LectureModel>(dto));
        }

        #endregion

        #region Insert

        public static IEnumerable<IEnumerable<object>> InsertTestCaseGenerator()
        {
            yield return new object[]
            {
                new LectureAddModel("Test Lecture", 2),
                new LectureDto { Id = 0, Name = "Test Lecture", CourseId = 2 }
            };
        }

        [TestCaseSource(nameof(InsertTestCaseGenerator))]
        public async Task InsertTest(LectureAddModel model, LectureDto expected)
        {
            // Arrange
            Mock<IMapper> mapperMock = new();
            mapperMock.Setup(mapper => mapper.Map<LectureDto>(model)).Returns(expected).Verifiable();
            Mock<ILectureService> serviceMock = new();
            serviceMock.Setup(service => service.AddLecture(expected)).Verifiable();
            var loggerMock = new Mock<ILogger<LectureController>>();
            var controller = new LectureController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

            // Act
            await controller.Insert(model);

            // Assert
            serviceMock.Verify();
            serviceMock.VerifyNoOtherCalls();
            mapperMock.Verify();
        }

        #endregion
        
        #region Update

        public static IEnumerable<IEnumerable<object>> UpdateTestCaseGenerator()
        {
            yield return new object[]
            {
                new LectureUpdateModel(1, "Test Lecture"),
                new LectureDto { Id = 1, Name = "Test Lecture" }
            };
        }

        [TestCaseSource(nameof(UpdateTestCaseGenerator))]
        public async Task UpdateTest(LectureUpdateModel model, LectureDto expected)
        {
            // Arrange
            Mock<IMapper> mapperMock = new();
            mapperMock.Setup(mapper => mapper.Map<LectureDto>(model)).Returns(expected).Verifiable();
            Mock<ILectureService> serviceMock = new();
            serviceMock.Setup(service => service.UpdateLecture(expected)).Verifiable();
            var loggerMock = new Mock<ILogger<LectureController>>();
            var controller = new LectureController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

            // Act
            await controller.Update(model);

            // Assert
            serviceMock.Verify();
            serviceMock.VerifyNoOtherCalls();
            mapperMock.Verify();
        }

        #endregion

        #region Delete

        [Test]
        public async Task DeleteTest([Values(1)] int id)
        {
            // Arrange
            Mock<ILectureService> serviceMock = new();
            serviceMock.Setup(service => service.DeleteLecture(id)).Verifiable();
            var loggerMock = new Mock<ILogger<LectureController>>();
            var mapperMock = new Mock<IMapper>();
            var controller = new LectureController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

            // Act
            await controller.Delete(id);

            // Assert
            serviceMock.Verify();
            serviceMock.VerifyNoOtherCalls();
        }

        #endregion
    }
}
