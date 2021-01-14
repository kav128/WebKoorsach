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
    public class StudentServiceTests
    {
        public static IEnumerable<IEnumerable<object>> AddStudentTestCaseGenerator()
        {
            yield return new object[]
            {
                new StudentDto { Id = 0, FullName = "Student" },
                new Student { Id = 0, FullName = "Student", JournalRecords = Array.Empty<JournalRecord>() }
            };
        }

        [TestCaseSource(nameof(AddStudentTestCaseGenerator))]
        public async Task AddStudentTest(StudentDto studentDto, Student expected)
        {
            // Arrange
            Mock<ICrudRepository<Student>> repositoryMock = new();
            var loggerMock = new Mock<ILogger<StudentService>>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<Student?>(studentDto)).Returns(expected);
            var service = new StudentService(repositoryMock.Object, mapperMock.Object, loggerMock.Object);
            
            // Act
            int id = await service.AddStudent(studentDto);

            // Assert
            Assert.That(id, Is.EqualTo(expected.Id));
            repositoryMock.Verify(repository => repository.SaveAsync(expected, It.IsAny<CancellationToken>()));
            repositoryMock.VerifyNoOtherCalls();
        }

        public static IEnumerable<IEnumerable<object>> GetStudentTestCaseGenerator()
        {
            yield return new object[]
            {
                1, new Student { Id = 1, FullName = "Student", JournalRecords = Array.Empty<JournalRecord>() },
                new StudentDto { Id = 1, FullName = "Student" }
            };
        }

        [TestCaseSource(nameof(GetStudentTestCaseGenerator))]
        public async Task GetStudentTest(int id, Student? student, StudentDto? expected)
        {
            // Arrange
            Mock<ICrudRepository<Student>> repositoryMock = new();
            repositoryMock.Setup(repository => repository.GetByIdAsync(It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(student));
            var loggerMock = new Mock<ILogger<StudentService>>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<StudentDto?>(student)).Returns(expected);
            StudentService service = new(repositoryMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            StudentDto? studentDto = await service.GetStudent(id);
            
            // Assert
            Assert.That(studentDto, Is.EqualTo(expected));
            repositoryMock.Verify(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once());
            repositoryMock.VerifyNoOtherCalls();
        }
        
        public static IEnumerable<IEnumerable<object>> GetStudentTestCaseGeneratorException()
        {
            yield return new object[] { -1 };
            yield return new object[] { 0 };
        }

        [TestCaseSource(nameof(GetStudentTestCaseGeneratorException))]
        public void GetStudentTest_ThrowsException(int id)
        {
            // Arrange
            Mock<ICrudRepository<Student>> repositoryMock = new();
            repositoryMock.Setup(repository => repository.GetByIdAsync(It.IsAny<int>(),
                    It.IsAny<CancellationToken>())).ThrowsAsync(new IncorrectIdException());
            var loggerMock = new Mock<ILogger<StudentService>>();
            var mapperMock = new Mock<IMapper>();
            StudentService service = new(repositoryMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            async Task<StudentDto?> TestAction() => await service.GetStudent(id);
            
            // Assert
            Assert.That(TestAction, Throws.TypeOf<IncorrectIdException>());
        }

        public static IEnumerable<IEnumerable<object>> GetStudentsTestCaseGenerator()
        {
            yield return new object[]
            {
                Array.Empty<Student>(), Array.Empty<StudentDto>()
            };
            
            yield return new object[]
            {
                new[]
                {
                    new Student { Id = 1, FullName = "Student 1", JournalRecords = Array.Empty<JournalRecord>() },
                    new Student { Id = 2, FullName = "Student 2", JournalRecords = Array.Empty<JournalRecord>() }
                },
                new[]
                {
                    new StudentDto { Id = 1, FullName = "Student 1" },
                    new StudentDto { Id = 2, FullName = "Student 2" }
                }
            };
        }

        [TestCaseSource(nameof(GetStudentsTestCaseGenerator))]
        public async Task GetStudentsTest(IEnumerable<Student> students, IList<StudentDto> expected)
        {
            // Arrange
            Mock<ICrudRepository<Student>> repositoryMock = new();
            repositoryMock.Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(students));
            var loggerMock = new Mock<ILogger<StudentService>>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<IList<StudentDto>>(students)).Returns(expected);
            StudentService service = new(repositoryMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            IList<StudentDto> dtos = await service.GetStudents();

            // Assert
            Assert.That(dtos, Is.EquivalentTo(expected));
        }

        public static IEnumerable<IEnumerable<object>> UpdateStudentTestCaseGenerator()
        {
            yield return new object[]
            {
                new StudentDto { Id = 5, FullName = "Student" },
                new Student { Id = 5, FullName = "Student", JournalRecords = Array.Empty<JournalRecord>() }
            };
        }

        [TestCaseSource(nameof(UpdateStudentTestCaseGenerator))]
        public async Task UpdateStudentTest(StudentDto studentDto, Student expected)
        {
            // Arrange
            Mock<ICrudRepository<Student>> repositoryMock = new();
            repositoryMock.Setup(repository => repository.SaveAsync(expected,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expected.Id));
            var loggerMock = new Mock<ILogger<StudentService>>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<Student>(studentDto)).Returns(expected);
            StudentService service = new(repositoryMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            int id = await service.UpdateStudent(studentDto);

            // Assert
            Assert.That(id, Is.EqualTo(expected.Id));
            repositoryMock.Verify(repository => repository.SaveAsync(expected, It.IsAny<CancellationToken>()));
            repositoryMock.VerifyNoOtherCalls();
        }

        public static IEnumerable<IEnumerable<object>> DeleteStudentTestCaseGenerator()
        {
            yield return new object[] { 1 };
        }

        [TestCaseSource(nameof(DeleteStudentTestCaseGenerator))]
        public async Task DeleteStudentTest(int id)
        {
            // Arrange
            Mock<ICrudRepository<Student>> repositoryMock = new();
            var loggerMock = new Mock<ILogger<StudentService>>();
            var mapperMock = new Mock<IMapper>();
            StudentService service = new(repositoryMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            await service.DeleteStudent(id);

            // Assert
            repositoryMock.Verify(repository => repository.DeleteAsync(It.Is<Student>(student => student.Id == id),
                    It.IsAny<CancellationToken>()), Times.Once());
            repositoryMock.VerifyNoOtherCalls();
        }
    }
}
