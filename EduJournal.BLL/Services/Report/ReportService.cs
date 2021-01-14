using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using EduJournal.BLL.DTO;
using EduJournal.BLL.Services.Entity;

namespace EduJournal.BLL.Services.Report
{
    public class ReportService : IReportService
    {
        private readonly IJournalService _journalService;
        private readonly IStudentService _studentService;
        private readonly ILectureService _lectureService;
        private readonly ICourseService _courseService;

        public ReportService(IJournalService journalService, IStudentService studentService, ILectureService lectureService, ICourseService courseService)
        {
            _journalService = journalService;
            _studentService = studentService;
            _lectureService = lectureService;
            _courseService = courseService;
        }

        public async Task<ReportData> GetReportByLecture(LectureDto lecture)
        {
            CourseDto? courseDto = await _courseService.GetCourse(lecture.CourseId);
            if (courseDto is null) throw new Exception("TODO"); // TODO Implement custom exception type
            
            var reportHeader = new ReportHeader { Lecture = lecture.Name, Course = courseDto.Name, Student = null };

            IList<JournalRecordDto> journalRecords = await _journalService.GetRecords(lecture.Id);
            IList<int> studentIds = journalRecords.Select(dto => dto.StudentId)
                .Distinct()
                .OrderBy(i => i)
                .ToImmutableList();
            IList<StudentDto> students = await _studentService.GetStudentsByIds(studentIds);
            ReportRecord[] reportRecords = journalRecords.Select(dto => new ReportRecord
            {
                Attendance = dto.Attendance,
                Score = dto.Score,
                Student = students.First(studentDto => studentDto.Id == dto.StudentId).FullName,
                Lecture = null
            }).ToArray();

            
            double? attendancePercentage = reportRecords.IsEmpty() 
                ? null
                : reportRecords.Average(record => Convert.ToInt32(record.Attendance)) * 100;
            var report = new ReportData
            {
                Header = reportHeader,
                Records = reportRecords,
                AttendancePercentage = attendancePercentage,
                AverageScore = null
            };
            return report;
        }

        public async Task<ReportData> GetReportByStudent(StudentDto student, CourseDto course)
        {
            var reportHeader = new ReportHeader { Course = course.Name, Student = student.FullName, Lecture = null };

            IList<JournalRecordDto> journalRecords = await _journalService.GetRecords(0, student.Id, course.Id);

            IList<LectureDto> lectures = await _lectureService.GetAllByCourse(course.Id);

            ReportRecord[] reportRecords = journalRecords.Select(dto => new ReportRecord
            {
                Lecture = lectures.First(lectureDto => lectureDto.Id == dto.LectureId).Name,
                Attendance = dto.Attendance,
                Score = dto.Score,
                Student = null
            }).ToArray();

            double? attendancePercentage = reportRecords.IsEmpty() 
                ? null
                : reportRecords.Average(record => Convert.ToInt32(record.Attendance)) * 100;
            
            double? averageScore = reportRecords.IsEmpty()
                ? null
                : reportRecords.Average(record => record.Score);
            var report = new ReportData
            {
                Header = reportHeader,
                Records = reportRecords,
                AttendancePercentage = attendancePercentage,
                AverageScore = averageScore
            };
            return report;
        }
    }
}