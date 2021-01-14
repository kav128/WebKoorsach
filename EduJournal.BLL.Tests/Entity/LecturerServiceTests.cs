#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EduJournal.BLL.DTO;
using EduJournal.BLL.Services.Entity;
using EduJournal.DAL.Entities;
using EduJournal.DAL.Repositories.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EduJournal.BLL.Tests.Entity
{
    [TestFixture]
    public class LecturerServiceTests
    {
        public static IEnumerable<IEnumerable<object>> AddLecturerTestCaseGenerator()
        {
            yield return new object[]
            {
                new LecturerDto { FullName = "Lecturer" },
                new Lecturer { FullName = "Lecturer", Courses = new List<Course>() }
            };
        }

        [TestCaseSource(nameof(AddLecturerTestCaseGenerator))]
        public async Task AddLecturerTest(LecturerDto lecturerDto, Lecturer expected)
        {
            // Arrange
            Mock<ICrudRepository<Lecturer>> repositoryMock = new();
            var loggerMock = new Mock<ILogger<LecturerService>>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<Lecturer>(lecturerDto)).Returns(expected);
            var service = new LecturerService(repositoryMock.Object, mapperMock.Object, loggerMock.Object);
            
            // Act
            int id = await service.AddLecturer(lecturerDto);

            // Assert
            Assert.That(id, Is.EqualTo(expected.Id));
            repositoryMock.Verify(repository => repository.SaveAsync(expected, It.IsAny<CancellationToken>()));
            repositoryMock.VerifyNoOtherCalls();
        }

        public static IEnumerable<IEnumerable<object>> GetLecturerTestCaseGenerator()
        {
            yield return new object[]
            {
                1, new Lecturer { Id = 1, FullName = "Lecturer", Courses = Array.Empty<Course>() },
                new LecturerDto { Id = 1, FullName = "Lecturer", CourseIds = Array.Empty<int>() }
            };
        }

        [TestCaseSource(nameof(GetLecturerTestCaseGenerator))]
        public async Task GetLecturerTest(int id, Lecturer? lecturer, LecturerDto? expected)
        {
            // Arrange
            Mock<ICrudRepository<Lecturer>> repositoryMock = new();
            repositoryMock.Setup(repository => repository.GetByIdAsync(It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(lecturer));
            var loggerMock = new Mock<ILogger<LecturerService>>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<LecturerDto?>(lecturer)).Returns(expected);
            LecturerService service = new(repositoryMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            LecturerDto? lecturerDto = await service.GetLecturer(id);
            
            // Assert
            Assert.That(lecturerDto, Is.EqualTo(expected));
            repositoryMock.Verify(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once());
            repositoryMock.VerifyNoOtherCalls();
        }
        
        public static IEnumerable<IEnumerable<object>> GetLecturerTestCaseGeneratorException()
        {
            yield return new object[] { -1 };
            yield return new object[] { 0 };
        }

        [TestCaseSource(nameof(GetLecturerTestCaseGeneratorException))]
        public void GetLecturerTest_ThrowsException(int id)
        {
            // Arrange
            Mock<ICrudRepository<Lecturer>> repositoryMock = new();
            repositoryMock.Setup(repository => repository.GetByIdAsync(It.IsAny<int>(),
                It.IsAny<CancellationToken>())).ThrowsAsync(new IncorrectIdException());
            var loggerMock = new Mock<ILogger<LecturerService>>();
            var mapperMock = new Mock<IMapper>();
            LecturerService service = new(repositoryMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            async Task<LecturerDto?> TestAction() => await service.GetLecturer(id);
            
            // Assert
            Assert.That(TestAction, Throws.TypeOf<IncorrectIdException>());
        }

        public static IEnumerable<IEnumerable<object>> GetLecturersTestCaseGenerator()
        {
            yield return new object[]
            {
                Array.Empty<Lecturer>(), Array.Empty<LecturerDto>()
            };
            
            yield return new object[]
            {
                new[]
                {
                    new Lecturer { Id = 1, FullName = "Lecturer 1", Courses = Array.Empty<Course>() },
                    new Lecturer { Id = 2, FullName = "Lecturer 2", Courses = Array.Empty<Course>()  }
                },
                new[]
                {
                    new LecturerDto { Id = 1, FullName = "Lecturer 1", CourseIds = Array.Empty<int>() },
                    new LecturerDto { Id = 2, FullName = "Lecturer 2", CourseIds = Array.Empty<int>() }
                }
            };
        }

        [TestCaseSource(nameof(GetLecturersTestCaseGenerator))]
        public async Task GetLecturersTest(IEnumerable<Lecturer> lecturers, IList<LecturerDto> expected)
        {
            // Arrange
            Mock<ICrudRepository<Lecturer>> repositoryMock = new();
            repositoryMock.Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(lecturers));
            var loggerMock = new Mock<ILogger<LecturerService>>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<IList<LecturerDto>>(lecturers)).Returns(expected);
            LecturerService service = new(repositoryMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            IEnumerable<LecturerDto> dtos = await service.GetLecturers();

            // Assert
            Assert.That(dtos, Is.EquivalentTo(expected));
        }

        public static IEnumerable<IEnumerable<object>> UpdateLecturerTestCaseGenerator()
        {
            yield return new object[]
            {
                new LecturerDto { Id = 5, FullName = "Lecturer", CourseIds = Array.Empty<int>() },
                new Lecturer { Id = 5, FullName = "Lecturer", Courses = Array.Empty<Course>() }
            };
        }

        [TestCaseSource(nameof(UpdateLecturerTestCaseGenerator))]
        public async Task UpdateLecturerTest(LecturerDto lecturerDto, Lecturer expected)
        {
            // Arrange
            Mock<ICrudRepository<Lecturer>> repositoryMock = new();
            repositoryMock.Setup(repository => repository.SaveAsync(expected, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expected.Id));
            var loggerMock = new Mock<ILogger<LecturerService>>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<Lecturer>(lecturerDto)).Returns(expected);
            LecturerService service = new(repositoryMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            int id = await service.UpdateLecturer(lecturerDto);

            // Assert
            Assert.That(id, Is.EqualTo(expected.Id));
            repositoryMock.Verify(repository => repository.SaveAsync(expected, It.IsAny<CancellationToken>()));
            repositoryMock.VerifyNoOtherCalls();
        }

        [TestCase(1)]
        public async Task DeleteLecturerTest(int id)
        {
            // Arrange
            Mock<ICrudRepository<Lecturer>> repositoryMock = new();
            var loggerMock = new Mock<ILogger<LecturerService>>();
            var mapperMock = new Mock<IMapper>();
            LecturerService service = new(repositoryMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            await service.DeleteLecturer(id);

            // Assert
            repositoryMock.Verify(repository => repository.DeleteAsync(
                    It.Is<Lecturer>(lecturer => lecturer.Id == id), 
                    It.IsAny<CancellationToken>()), Times.Once());
            repositoryMock.VerifyNoOtherCalls();
        }
    }
}
