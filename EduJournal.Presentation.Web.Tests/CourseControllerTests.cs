using System;
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
    public class CourseControllerTests
    {
        #region GetAll

        public static IEnumerable<IEnumerable<object>> GetAllTestCaseGenerator()
        {
            yield return new object[]
            {
                new CourseDto[]
                {
                    new() { Id = 1, Name = "Test Course", LecturerId = 1, LectureIds = Array.Empty<int>() }
                },
                new CourseModel[] { new(1, "Test Course", 1, Array.Empty<int>()) }
            };
            yield return new object[]
            {
                new CourseDto[]
                {
                    new() { Id = 1, Name = "Test Course1", LecturerId = 1, LectureIds = Array.Empty<int>() },
                    new() { Id = 2, Name = "Test Course2", LecturerId = 2, LectureIds = new[] { 1, 2, 3, 4 } }
                },
                new CourseModel[]
                {
                    new(1, "Test Course 1", 1, Array.Empty<int>()),
                    new(2, "Test Course 2", 2, new[] { 1, 2, 3, 4 })
                }
            };
        }

        [TestCaseSource(nameof(GetAllTestCaseGenerator))]
        public async Task GetAllTest(IList<CourseDto> data, IList<CourseModel> expected)
        {
            // Arrange
            Mock<ICourseService> serviceMock = new();
            serviceMock.Setup(service => service.GetAll()).ReturnsAsync(() => data).Verifiable();
            Mock<IMapper> mapperMock = new();
            mapperMock.Setup(mapper => mapper.Map<IList<CourseModel>>(data)).Returns(expected).Verifiable();
            var loggerMock = new Mock<ILogger<CourseController>>();
            var controller = new CourseController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

            // Act
            IList<CourseModel> models = await controller.GetAll();

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
                new CourseDto[]
                {
                    new() { Id = 1, Name = "Test Course", LecturerId = 1, LectureIds = Array.Empty<int>() }
                },
                new CourseDto { Id = 1, Name = "Test Course", LecturerId = 1, LectureIds = Array.Empty<int>() },
                1,
                new CourseModel(1, "Test Course", 1, Array.Empty<int>())
            };
            yield return new object[]
            {
                new CourseDto[]
                {
                    new() { Id = 1, Name = "Test Course 1", LecturerId = 1, LectureIds = Array.Empty<int>() },
                    new() { Id = 2, Name = "Test Course 2", LecturerId = 2, LectureIds = new[] { 1, 2, 3, 4 } }
                },
                new CourseDto { Id = 2, Name = "Test Course2", LecturerId = 2, LectureIds = new[] { 1, 2, 3, 4 } },
                2,
                new CourseModel(2, "Test Course 2", 2, new[] { 1, 2, 3, 4 })
            };
            yield return new object[]
            {
                new CourseDto[]
                {
                    new() { Id = 1, Name = "Test Course1", LecturerId = 1, LectureIds = Array.Empty<int>() },
                    new() { Id = 2, Name = "Test Course2", LecturerId = 2, LectureIds = new[] { 1, 2, 3, 4 } }
                },
                null,
                3,
                null
            };
        }

        [TestCaseSource(nameof(GetByIdTestCaseGenerator))]
        public async Task GetByIdTest(IList<CourseDto> data, CourseDto dto, int id, CourseModel expected)
        {
            // Arrange
            Mock<ICourseService> serviceMock = new();
            serviceMock.Setup(service => service.GetCourse(id)).ReturnsAsync(() => dto).Verifiable();
            Mock<IMapper> mapperMock = new();
            mapperMock.Setup(mapper => mapper.Map<CourseModel>(dto)).Returns(expected);
            var loggerMock = new Mock<ILogger<CourseController>>();
            var controller = new CourseController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

            // Act
            ActionResult<CourseModel> actionResult = await controller.GetById(id);
            CourseModel model = actionResult.Value;

            // Assert
            Assert.That(model, Is.EqualTo(expected));
            serviceMock.Verify();
            serviceMock.VerifyNoOtherCalls();
            if (dto is not null) mapperMock.Verify(mapper => mapper.Map<CourseModel>(dto));
        }

        #endregion

        #region Insert

        public static IEnumerable<IEnumerable<object>> InsertTestCaseGenerator()
        {
            yield return new object[]
            {
                new CourseAddModel("Test Course", 1),
                new CourseDto { Id = 0, Name = "Test Course", LecturerId = 1, LectureIds = Array.Empty<int>() }
            };
        }

        [TestCaseSource(nameof(InsertTestCaseGenerator))]
        public async Task InsertTest(CourseAddModel model, CourseDto expected)
        {
            // Arrange
            Mock<IMapper> mapperMock = new();
            mapperMock.Setup(mapper => mapper.Map<CourseDto>(model)).Returns(expected).Verifiable();
            Mock<ICourseService> serviceMock = new();
            serviceMock.Setup(service => service.AddCourse(expected)).Verifiable();
            var loggerMock = new Mock<ILogger<CourseController>>();
            var controller = new CourseController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

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
                new CourseUpdateModel(1, "Test Course", 1),
                new CourseDto { Id = 1, Name = "Test Course", LecturerId = 1, LectureIds = Array.Empty<int>() }
            };
        }

        [TestCaseSource(nameof(UpdateTestCaseGenerator))]
        public async Task UpdateTest(CourseUpdateModel model, CourseDto expected)
        {
            // Arrange
            Mock<IMapper> mapperMock = new();
            mapperMock.Setup(mapper => mapper.Map<CourseDto>(model)).Returns(expected).Verifiable();
            Mock<ICourseService> serviceMock = new();
            serviceMock.Setup(service => service.UpdateCourse(expected)).Verifiable();
            var loggerMock = new Mock<ILogger<CourseController>>();
            var controller = new CourseController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

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
            Mock<ICourseService> serviceMock = new();
            serviceMock.Setup(service => service.DeleteCourse(id)).Verifiable();
            var loggerMock = new Mock<ILogger<CourseController>>();
            var mapperMock = new Mock<IMapper>();
            var controller = new CourseController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

            // Act
            await controller.Delete(id);

            // Assert
            serviceMock.Verify();
            serviceMock.VerifyNoOtherCalls();
        }

        #endregion
    }
}
