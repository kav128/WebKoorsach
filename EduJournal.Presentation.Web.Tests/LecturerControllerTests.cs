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
    public class LecturerControllerTests
    {
        #region GetAll

        public static IEnumerable<IEnumerable<object>> GetAllTestCaseGenerator()
        {
            yield return new object[]
            {
                new LecturerDto[]
                {
                    new() { Id = 1, FullName = "Lecturer 1", CourseIds = new[] { 1, 2, 5 } }
                },
                new LecturerModel[] { new(1, "Lecturer 1", "test@localhost", new[] { 1, 2, 5 }) }
            };
            yield return new object[]
            {
                new LecturerDto[]
                {
                    new() { Id = 1, FullName = "Lecturer 1", CourseIds = new[] { 1, 2, 5 } },
                    new() { Id = 2, FullName = "Lecturer 2", CourseIds = Array.Empty<int>() }
                },
                new LecturerModel[]
                {
                    new(1, "Lecturer 1", "test@localhost", new[] { 1, 2, 5 }),
                    new(2, "Lecturer 2", "test@localhost", Array.Empty<int>())
                }
            };
        }

        [TestCaseSource(nameof(GetAllTestCaseGenerator))]
        public async Task GetAllTest(IList<LecturerDto> data, IList<LecturerModel> expected)
        {
            // Arrange
            Mock<ILecturerService> serviceMock = new();
            serviceMock.Setup(service => service.GetLecturers()).ReturnsAsync(() => data).Verifiable();
            Mock<IMapper> mapperMock = new();
            mapperMock.Setup(mapper => mapper.Map<IList<LecturerModel>>(data)).Returns(expected).Verifiable();
            var loggerMock = new Mock<ILogger<LecturerController>>();
            var controller = new LecturerController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

            // Act
            IList<LecturerModel> models = await controller.GetAll();

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
                new LecturerDto[]
                {
                    new() { Id = 1, FullName = "Lecturer 1", CourseIds = new[] { 1, 2, 5 } }
                },
                new LecturerDto { Id = 1, FullName = "Lecturer 1", CourseIds = new[] { 1, 2, 5 } },
                1,
                new LecturerModel(1, "Lecturer 1", "test@localhost", new[] { 1, 2, 5 })
            };
            yield return new object[]
            {
                new LecturerDto[]
                {
                    new() { Id = 1, FullName = "Lecturer 1", CourseIds = new[] { 1, 2, 5 } },
                    new() { Id = 2, FullName = "Lecturer 2", CourseIds = Array.Empty<int>() }
                },
                new LecturerDto { Id = 2, FullName = "Lecturer 2", CourseIds = Array.Empty<int>() },
                2,
                new LecturerModel(2, "Lecturer 2", "test@localhost", Array.Empty<int>())
            };
            yield return new object[]
            {
                new LecturerDto[]
                {
                    new() { Id = 1, FullName = "Lecturer 1", CourseIds = new[] { 1, 2, 5 } },
                    new() { Id = 2, FullName = "Lecturer 2", CourseIds = Array.Empty<int>() }
                },
                null,
                3,
                null
            };
        }

        [TestCaseSource(nameof(GetByIdTestCaseGenerator))]
        public async Task GetByIdTest(IList<LecturerDto> data, LecturerDto dto, int id, LecturerModel expected)
        {
            // Arrange
            Mock<ILecturerService> serviceMock = new();
            serviceMock.Setup(service => service.GetLecturer(id)).ReturnsAsync(() => dto).Verifiable();
            Mock<IMapper> mapperMock = new();
            mapperMock.Setup(mapper => mapper.Map<LecturerModel>(dto)).Returns(expected);
            var loggerMock = new Mock<ILogger<LecturerController>>();
            var controller = new LecturerController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

            // Act
            ActionResult<LecturerModel> actionResult = await controller.GetById(id);
            LecturerModel model = actionResult.Value;

            // Assert
            Assert.That(model, Is.EqualTo(expected));
            serviceMock.Verify();
            serviceMock.VerifyNoOtherCalls();
            if (dto is not null) mapperMock.Verify(mapper => mapper.Map<LecturerModel>(dto));
        }

        #endregion

        #region Insert

        public static IEnumerable<IEnumerable<object>> InsertTestCaseGenerator()
        {
            yield return new object[]
            {
                new LecturerAddModel("Test Lecturer", "test@localhost"),
                new LecturerDto { Id = 0, FullName = "Test Lecturer", CourseIds = Array.Empty<int>() }
            };
        }

        [TestCaseSource(nameof(InsertTestCaseGenerator))]
        public async Task InsertTest(LecturerAddModel model, LecturerDto expected)
        {
            // Arrange
            Mock<IMapper> mapperMock = new();
            mapperMock.Setup(mapper => mapper.Map<LecturerDto>(model)).Returns(expected).Verifiable();
            Mock<ILecturerService> serviceMock = new();
            serviceMock.Setup(service => service.AddLecturer(expected)).Verifiable();
            var loggerMock = new Mock<ILogger<LecturerController>>();
            var controller = new LecturerController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

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
                new LecturerUpdateModel(1, "Test Lecturer", "test@localhost"),
                new LecturerDto { Id = 1, FullName = "Test Lecturer", CourseIds = Array.Empty<int>() }
            };
        }

        [TestCaseSource(nameof(UpdateTestCaseGenerator))]
        public async Task UpdateTest(LecturerUpdateModel model, LecturerDto expected)
        {
            // Arrange
            Mock<IMapper> mapperMock = new();
            mapperMock.Setup(mapper => mapper.Map<LecturerDto>(model)).Returns(expected).Verifiable();
            Mock<ILecturerService> serviceMock = new();
            serviceMock.Setup(service => service.UpdateLecturer(expected)).Verifiable();
            var loggerMock = new Mock<ILogger<LecturerController>>();
            var controller = new LecturerController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

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
            Mock<ILecturerService> serviceMock = new();
            serviceMock.Setup(service => service.DeleteLecturer(id)).Verifiable();
            var loggerMock = new Mock<ILogger<LecturerController>>();
            var mapperMock = new Mock<IMapper>();
            var controller = new LecturerController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

            // Act
            await controller.Delete(id);

            // Assert
            serviceMock.Verify();
            serviceMock.VerifyNoOtherCalls();
        }

        #endregion
    }
}
