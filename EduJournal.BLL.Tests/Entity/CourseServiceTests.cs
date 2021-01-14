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
    public class CourseServiceTests
    {
        public static IEnumerable<IEnumerable<object>> AddCourseTestCaseGenerator()
        {
            yield return new object[]
            {
                new CourseDto { Name = "Course", LecturerId = 1, LectureIds = Array.Empty<int>()},
                new Course { Name = "Course", LecturerId = 1, Lectures = Array.Empty<Lecture>() }
            };
        }

        [TestCaseSource(nameof(AddCourseTestCaseGenerator))]
        public async Task AddCourseTest(CourseDto courseDto, Course expected)
        {
            // Arrange
            Mock<ICrudRepository<Course>> repositoryMock = new();
            var loggerMock = new Mock<ILogger<CourseService>>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<Course>(courseDto)).Returns(expected);
            var service = new CourseService(repositoryMock.Object, mapperMock.Object, loggerMock.Object);
            
            // Act
            int id = await service.AddCourse(courseDto);

            // Assert
            Assert.That(id, Is.EqualTo(expected.Id));
            repositoryMock.Verify(repository => repository.SaveAsync(expected, It.IsAny<CancellationToken>()));
            repositoryMock.VerifyNoOtherCalls();
        }

        public static IEnumerable<IEnumerable<object>> GetCourseTestCaseGenerator()
        {
            yield return new object[]
            {
                1, new Course { Id = 1, LecturerId = 5, Name = "Course", Lecturer = new Lecturer { Id = 5 }, Lectures = Array.Empty<Lecture>()},
                new CourseDto { Id = 1, LecturerId = 5, Name = "Course", LectureIds = Array.Empty<int>() }
            };
        }

        [TestCaseSource(nameof(GetCourseTestCaseGenerator))]
        public async Task GetCourseTest(int id, Course? course, CourseDto? expected)
        {
            // Arrange
            Mock<ICrudRepository<Course>> repositoryMock = new();
            repositoryMock.Setup(repository => repository.GetByIdAsync(It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(course));
            var loggerMock = new Mock<ILogger<CourseService>>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<CourseDto?>(course)).Returns(expected);
            CourseService service = new(repositoryMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            CourseDto? courseDto = await service.GetCourse(id);
            
            // Assert
            Assert.That(courseDto, Is.EqualTo(expected));
            repositoryMock.Verify(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once());
            repositoryMock.VerifyNoOtherCalls();
        }

        public static IEnumerable<IEnumerable<object>> GetCourseTestCaseGeneratorException()
        {
            yield return new object[] { -1 };
            yield return new object[] { 0 };
        }

        [TestCaseSource(nameof(GetCourseTestCaseGeneratorException))]
        public void GetCourseTest_ThrowsException(int id)
        {
            // Arrange
            Mock<ICrudRepository<Course>> repositoryMock = new();
            repositoryMock.Setup(repository => repository.GetByIdAsync(It.IsAny<int>(),
                    It.IsAny<CancellationToken>())).ThrowsAsync(new IncorrectIdException());
            var loggerMock = new Mock<ILogger<CourseService>>();
            var mapperMock = new Mock<IMapper>();
            CourseService service = new(repositoryMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            async Task<CourseDto?> TestAction() => await service.GetCourse(id);
            
            // Assert
            Assert.That(TestAction, Throws.TypeOf<IncorrectIdException>());
        }

        public static IEnumerable<IEnumerable<object>> UpdateCourseTestCaseGenerator()
        {
            yield return new object[]
            {
                new CourseDto { Id = 5, Name = "Course", LecturerId = 2, LectureIds = Array.Empty<int>() },
                new Course { Id = 5, Name = "Course", LecturerId = 2, Lectures = Array.Empty<Lecture>(), Lecturer = new Lecturer { Id = 2 }}
            };
        }

        [TestCaseSource(nameof(UpdateCourseTestCaseGenerator))]
        public async Task UpdateCourseTest(CourseDto courseDto, Course expected)
        {
            // Arrange
            Mock<ICrudRepository<Course>> repositoryMock = new();
            repositoryMock.Setup(repository => repository.SaveAsync(expected, It.IsAny<CancellationToken>())).Returns(Task.FromResult(expected.Id));
            var loggerMock = new Mock<ILogger<CourseService>>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<Course>(courseDto)).Returns(expected);
            CourseService service = new(repositoryMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            int id = await service.UpdateCourse(courseDto);

            // Assert
            Assert.That(id, Is.EqualTo(expected.Id));
            repositoryMock.Verify(repository => repository.SaveAsync(expected, It.IsAny<CancellationToken>()));
            repositoryMock.VerifyNoOtherCalls();
        }

        public static IEnumerable<IEnumerable<object>> DeleteCourseTestCaseGenerator()
        {
            yield return new object[] { 1 };
        }

        [TestCaseSource(nameof(DeleteCourseTestCaseGenerator))]
        public async Task DeleteCourseTest(int id)
        {
            // Arrange
            Mock<ICrudRepository<Course>> repositoryMock = new();
            var loggerMock = new Mock<ILogger<CourseService>>();
            var mapperMock = new Mock<IMapper>();
            CourseService service = new(repositoryMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            await service.DeleteCourse(id);

            // Assert
            repositoryMock.Verify(repository =>
                    repository.DeleteAsync(It.Is<Course>(course => course.Id == id), It.IsAny<CancellationToken>()),
                Times.Once());
            repositoryMock.VerifyNoOtherCalls();
        }
    }
}
