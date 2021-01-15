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
    public class StudentControllerTests
    {
        #region GetAll

        public static IEnumerable<IEnumerable<object>> GetAllTestCaseGenerator()
        {
            yield return new object[]
            {
                new StudentDto[]
                {
                    new() { Id = 1, FullName = "Student 1" }
                },
                new StudentModel[] { new(1, "Student 1", "test@localhost") }
            };
            yield return new object[]
            {
                new StudentDto[]
                {
                    new() { Id = 1, FullName = "Student 1" },
                    new() { Id = 2, FullName = "Student 2" }
                },
                new StudentModel[]
                {
                    new(1, "Student 1", "test@localhost"),
                    new(2, "Student 2", "test@localhost")
                }
            };
        }

        [TestCaseSource(nameof(GetAllTestCaseGenerator))]
        public async Task GetAllTest(IList<StudentDto> data, IList<StudentModel> expected)
        {
            // Arrange
            Mock<IStudentService> serviceMock = new();
            serviceMock.Setup(service => service.GetStudents()).ReturnsAsync(() => data).Verifiable();
            Mock<IMapper> mapperMock = new();
            mapperMock.Setup(mapper => mapper.Map<IList<StudentModel>>(data)).Returns(expected).Verifiable();
            var loggerMock = new Mock<ILogger<StudentController>>();
            var controller = new StudentController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

            // Act
            IList<StudentModel> models = await controller.GetAll();

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
                new StudentDto[]
                {
                    new() { Id = 1, FullName = "Student 1" }
                },
                new StudentDto { Id = 1, FullName = "Student 1" },
                1,
                new StudentModel(1, "Student 1", "test@localhost")
            };
            yield return new object[]
            {
                new StudentDto[]
                {
                    new() { Id = 1, FullName = "Student 1" },
                    new() { Id = 2, FullName = "Student 2" }
                },
                new StudentDto { Id = 2, FullName = "Student 2" },
                2,
                new StudentModel(2, "Student 2", "test@localhost")
            };
            yield return new object[]
            {
                new StudentDto[]
                {
                    new() { Id = 1, FullName = "Student 1" },
                    new() { Id = 2, FullName = "Student 2" }
                },
                null,
                3,
                null
            };
        }

        [TestCaseSource(nameof(GetByIdTestCaseGenerator))]
        public async Task GetByIdTest(IList<StudentDto> data, StudentDto dto, int id, StudentModel expected)
        {
            // Arrange
            Mock<IStudentService> serviceMock = new();
            serviceMock.Setup(service => service.GetStudent(id)).ReturnsAsync(() => dto).Verifiable();
            Mock<IMapper> mapperMock = new();
            mapperMock.Setup(mapper => mapper.Map<StudentModel>(dto)).Returns(expected);
            var loggerMock = new Mock<ILogger<StudentController>>();
            var controller = new StudentController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

            // Act
            ActionResult<StudentModel> actionResult = await controller.GetById(id);
            StudentModel model = actionResult.Value;

            // Assert
            Assert.That(model, Is.EqualTo(expected));
            serviceMock.Verify();
            serviceMock.VerifyNoOtherCalls();
            if (dto is not null) mapperMock.Verify(mapper => mapper.Map<StudentModel>(dto));
        }

        #endregion

        #region Insert

        public static IEnumerable<IEnumerable<object>> InsertTestCaseGenerator()
        {
            yield return new object[]
            {
                new StudentAddModel("Test Student", "test@localhost"),
                new StudentDto { Id = 0, FullName = "Test Student" }
            };
        }

        [TestCaseSource(nameof(InsertTestCaseGenerator))]
        public async Task InsertTest(StudentAddModel model, StudentDto expected)
        {
            // Arrange
            Mock<IMapper> mapperMock = new();
            mapperMock.Setup(mapper => mapper.Map<StudentDto>(model)).Returns(expected).Verifiable();
            Mock<IStudentService> serviceMock = new();
            serviceMock.Setup(service => service.AddStudent(expected)).Verifiable();
            var loggerMock = new Mock<ILogger<StudentController>>();
            var controller = new StudentController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

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
                new StudentUpdateModel(1, "Test Student", "test@localhost"),
                new StudentDto { Id = 1, FullName = "Test Student" }
            };
        }

        [TestCaseSource(nameof(UpdateTestCaseGenerator))]
        public async Task UpdateTest(StudentUpdateModel model, StudentDto expected)
        {
            // Arrange
            Mock<IMapper> mapperMock = new();
            mapperMock.Setup(mapper => mapper.Map<StudentDto>(model)).Returns(expected).Verifiable();
            Mock<IStudentService> serviceMock = new();
            serviceMock.Setup(service => service.UpdateStudent(expected)).Verifiable();
            var loggerMock = new Mock<ILogger<StudentController>>();
            var controller = new StudentController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

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
            Mock<IStudentService> serviceMock = new();
            serviceMock.Setup(service => service.DeleteStudent(id)).Verifiable();
            var loggerMock = new Mock<ILogger<StudentController>>();
            var mapperMock = new Mock<IMapper>();
            var controller = new StudentController(serviceMock.Object, loggerMock.Object, mapperMock.Object);

            // Act
            await controller.Delete(id);

            // Assert
            serviceMock.Verify();
            serviceMock.VerifyNoOtherCalls();
        }

        #endregion
    }
}
