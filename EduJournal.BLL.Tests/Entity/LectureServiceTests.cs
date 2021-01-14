#nullable enable
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
    public class LectureServiceTests
    {
        public static IEnumerable<IEnumerable<object>> AddLectureTestCaseGenerator()
        {
            yield return new object[]
            {
                new LectureDto { Name = "Lecture", CourseId = 1 },
                new Lecture { Name = "Lecture", CourseId = 1 }
            };
        }

        [TestCaseSource(nameof(AddLectureTestCaseGenerator))]
        public async Task AddLectureTest(LectureDto lectureDto, Lecture expected)
        {
            // Arrange
            Mock<ICrudRepository<Lecture>> repositoryMock = new();
            var loggerMock = new Mock<ILogger<LectureService>>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<Lecture>(lectureDto)).Returns(expected);
            var service = new LectureService(repositoryMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            int id = await service.AddLecture(lectureDto);

            // Assert
            Assert.That(id, Is.EqualTo(expected.Id));
            repositoryMock.Verify(repository => repository.SaveAsync(expected, It.IsAny<CancellationToken>()));
            repositoryMock.VerifyNoOtherCalls();
        }

        public static IEnumerable<IEnumerable<object>> GetLectureTestCaseGenerator()
        {
            yield return new object[]
            {
                1, new Lecture { Id = 1, CourseId = 5, Name = "Lecture" },
                new LectureDto { Id = 1, CourseId = 5, Name = "Lecture" }
            };
        }

        [TestCaseSource(nameof(GetLectureTestCaseGenerator))]
        public async Task GetLectureTest(int id, Lecture? lecture, LectureDto? expected)
        {
            // Arrange
            Mock<ICrudRepository<Lecture>> repositoryMock = new();
            repositoryMock.Setup(repository => repository.GetByIdAsync(It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(lecture));
            var loggerMock = new Mock<ILogger<LectureService>>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<LectureDto?>(lecture)).Returns(expected);
            LectureService service = new(repositoryMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            LectureDto? lectureDto = await service.GetLecture(id);

            // Assert
            Assert.That(lectureDto, Is.EqualTo(expected));
            repositoryMock.Verify(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once());
            repositoryMock.VerifyNoOtherCalls();
        }
        
        public static IEnumerable<IEnumerable<object>> GetLectureTestCaseGeneratorException()
        {
            yield return new object[] { -1 };
            yield return new object[] { 0 };
        }

        [TestCaseSource(nameof(GetLectureTestCaseGeneratorException))]
        public void GetLectureTest_ThrowsException(int id)
        {
            // Arrange
            Mock<ICrudRepository<Lecture>> repositoryMock = new();
            repositoryMock.Setup(repository => repository.GetByIdAsync(It.IsAny<int>(),
                    It.IsAny<CancellationToken>())).ThrowsAsync(new IncorrectIdException());
            var mapperMock = new Mock<IMapper>();
            LectureService service = new(repositoryMock.Object, mapperMock.Object, new Mock<ILogger<LectureService>>().Object);

            // Act
            async Task<LectureDto?> TestAction() => await service.GetLecture(id);
            
            // Assert
            Assert.That(TestAction, Throws.TypeOf<IncorrectIdException>());
        }

        public static IEnumerable<IEnumerable<object>> UpdateLectureTestCaseGenerator()
        {
            yield return new object[]
            {
                new LectureDto { Id = 5, Name = "Lecture", CourseId = 2 },
                new Lecture { Id = 5, Name = "Lecture", CourseId = 2 }
            };
        }

        [TestCaseSource(nameof(UpdateLectureTestCaseGenerator))]
        public async Task UpdateLectureTest(LectureDto lectureDto, Lecture expected)
        {
            // Arrange
            Mock<ICrudRepository<Lecture>> repositoryMock = new();
            repositoryMock.Setup(repository => repository.SaveAsync(expected,
                    It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(expected.Id));
            var loggerMock = new Mock<ILogger<LectureService>>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<Lecture>(lectureDto)).Returns(expected);
            LectureService service = new(repositoryMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            int id = await service.UpdateLecture(lectureDto);

            // Assert
            Assert.That(id, Is.EqualTo(expected.Id));
            repositoryMock.Verify(repository => repository.SaveAsync(expected, It.IsAny<CancellationToken>()));
            repositoryMock.VerifyNoOtherCalls();
        }

        public static IEnumerable<IEnumerable<object>> DeleteLectureTestCaseGenerator()
        {
            yield return new object[] { 1 };
        }

        [TestCaseSource(nameof(DeleteLectureTestCaseGenerator))]
        public async Task DeleteLectureTest(int id)
        {
            // Arrange
            Mock<ICrudRepository<Lecture>> repositoryMock = new();
            var loggerMock = new Mock<ILogger<LectureService>>();
            var mapperMock = new Mock<IMapper>();
            LectureService service = new(repositoryMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            await service.DeleteLecture(id);

            // Assert
            repositoryMock.Verify(repository => repository.DeleteAsync(It.Is<Lecture>(lecture => lecture.Id == id),
                    It.IsAny<CancellationToken>()),
                Times.Once());
            repositoryMock.VerifyNoOtherCalls();
        }
    }
}
