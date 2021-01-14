#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using EduJournal.BLL.DTO;
using EduJournal.BLL.Services.Report;
using EduJournal.DAL.Entities;
using EduJournal.DAL.Exceptions;
using EduJournal.DAL.Repositories.Generic;
using Microsoft.Extensions.Logging;

namespace EduJournal.BLL.Services.Entity
{
    /// <summary>
    /// A CRUD service for lectures.
    /// </summary>
    public class JournalService : IJournalService
    {
        private readonly ICrudRepository<JournalRecord> _repository;
        private readonly IMessageSenderFactory _senderFactory;
        private readonly ICourseService _courseService;
        private readonly ILectureService _lectureService;
        private readonly IStudentService _studentService;
        private readonly ILecturerService _lecturerService;
        private readonly ILogger<JournalService> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="LectureService"/>.
        /// </summary>
        /// <param name="repository">Repository which performs CRUD operations.</param>
        /// <param name="senderFactory">A factory for creating message senders.</param>
        /// <param name="courseService">Course service.</param>
        /// <param name="lectureService">Lecture service.</param>
        /// <param name="studentService">Student service.</param>
        /// <param name="lecturerService">Lecturer service.</param>
        /// <param name="logger">A logger.</param>
        public JournalService(ICrudRepository<JournalRecord> repository,
            IMessageSenderFactory senderFactory,
            ICourseService courseService,
            ILectureService lectureService,
            IStudentService studentService,
            ILecturerService lecturerService,
            ILogger<JournalService> logger)
        {
            _repository = repository;
            _senderFactory = senderFactory;
            _courseService = courseService;
            _lectureService = lectureService;
            _studentService = studentService;
            _lecturerService = lecturerService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task SaveRecord(JournalRecordDto journalRecordDto)
        {
            LectureDto? lecture = await _lectureService.GetLecture(journalRecordDto.LectureId);
            if (lecture is null)
                throw new ReferenceNotFoundException("lecture", "Lecture with specified id does not exist.");
            StudentDto? student = await _studentService.GetStudent(journalRecordDto.StudentId);
            if (student is null)
                throw new ReferenceNotFoundException("student", "Student with specified id does not exist");

            _logger.LogInformation($"Saving journal record with dto {journalRecordDto}");
            IEnumerable<JournalRecord> filteredResult;
            try
            {
                filteredResult = await _repository.GetFilteredAsync(default,
                    record => record.LectureId == journalRecordDto.LectureId,
                    record => record.StudentId == journalRecordDto.StudentId);
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to get journal records");
                throw new UnexpectedDataException("Unable to get journal records", e);
            }
            int id = filteredResult.FirstOrDefault()?.Id ?? default;
            
            IMapper mapper = new MapperConfiguration(expression => expression
                .CreateMap<JournalRecordDto, JournalRecord>()
                .ForMember("Id", c => c.MapFrom(_ => id)))
                .CreateMapper();
            JournalRecord journalRecord = mapper.Map<JournalRecordDto, JournalRecord>(journalRecordDto);
            _logger.LogInformation($"Saving journal record {journalRecord}");
            try
            {
                await _repository.SaveAsync(journalRecord);
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to save journal records");
                throw new UnexpectedDataException("Unable to save journal records", e);
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogError(e, "Unexpected behavior. Unable to save journal record");
                throw new UnexpectedDataException("Unexpected behavior. Unable to save journal record", e);
            }

            IList<JournalRecordDto> records = await GetRecords(0, journalRecord.StudentId, lecture.CourseId);
            
            int missedLectures = records.Count(dto => !dto.Attendance);
            if (!journalRecordDto.Attendance && missedLectures > 3)
            {
                CourseDto? course = await _courseService.GetCourse(lecture.CourseId);
                if (course is null)
                    throw new UnexpectedDataException($"Unexpected behavior. Course with id {lecture.CourseId} does not exist");
                LecturerDto? lecturer = await _lecturerService.GetLecturer(course.LecturerId);
                if (lecturer is null)
                    throw new UnexpectedDataException($"Unexpected behavior. Lecturer with id {course.LecturerId} does not exist");
                
                IMessageSender emailSender = _senderFactory.GetEmailSender();
                emailSender.Send(
                    $"Student {student.FullName} missed {missedLectures} lectures in course '{course.Name}'!",
                    lecturer.Email);
                emailSender.Send(
                    $"You missed {missedLectures} lectures in course '{course.Name}'!",
                    student.Email);
            }

            double averageScore = records.Average(dto => dto.Score);
            if (journalRecordDto.Score < 4 && averageScore < 4)
            {
                CourseDto? course = await _courseService.GetCourse(lecture.CourseId);
                if (course is null)
                    throw new UnexpectedDataException($"Unexpected behavior. Course with id {lecture.CourseId} does not exist");

                IMessageSender smsSender = _senderFactory.GetSmsSender();
                smsSender.Send($"Your average mark in course '{course.Name}' is {averageScore}.", "+78005553535");
            }
        }

        /// <inheritdoc />
        public async Task<IList<JournalRecordDto>> GetRecords(int lectureId = default, int studentId = default, int courseId = default)
        {
            List<Expression<Func<JournalRecord, bool>>> expressions = new();
            if (lectureId != default) expressions.Add(record => record.LectureId == lectureId);
            if (studentId != default) expressions.Add(record => record.StudentId == studentId);
            if (lectureId == default && courseId != default) expressions.Add(record => record.Lecture.Course.Id == courseId);

            try
            {
                IEnumerable<JournalRecord> records = await _repository.GetFilteredAsync(default, expressions.ToArray());
                IMapper mapper = new MapperConfiguration(expression => expression
                        .CreateMap<JournalRecord, JournalRecordDto>())
                    .CreateMapper();
                return records.Select(record => mapper.Map<JournalRecord, JournalRecordDto>(record)).ToList();
            }
            catch (DataException e)
            {
                _logger.LogError(e, "Unable to save journal records");
                throw new UnexpectedDataException("Unable to save journal records", e);
            }
        }
    }
}
