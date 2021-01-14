using System;
using System.Threading.Tasks;
using EduJournal.BLL.DTO;
using EduJournal.BLL.Services.Entity;
using EduJournal.BLL.Services.Report;
using EduJournal.BLL.Services.Report.Formats;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EduJournal.Presentation.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IFormatterManager _formatterManager;
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly ILectureService _lectureService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportService reportService,
            IFormatterManager formatterManager,
            IStudentService studentService,
            ICourseService courseService,
            ILectureService lectureService,
            ILogger<ReportController> logger)
        {
            _reportService = reportService;
            _formatterManager = formatterManager;
            _studentService = studentService;
            _courseService = courseService;
            _lectureService = lectureService;
            _logger = logger;
        }

        [HttpGet("student.{format}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> GetReportByStudent(string format, int studentId, int courseId)
        {
            if (studentId <= 0) ModelState.AddModelError(nameof(studentId), "'studentId' must be positive");
            if (courseId <= 0) ModelState.AddModelError(nameof(courseId), "'courseId' must be positive");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            StudentDto? studentDto = await _studentService.GetStudent(studentId);
            if (studentDto is null) return BadRequest($"Student with id {studentId} does not exist.");
            CourseDto? courseDto = await _courseService.GetCourse(courseId);
            if (courseDto is null) return BadRequest($"Course with id {courseId} does not exist");

            IReportFormatter formatter;
            try
            {
                formatter = _formatterManager.GetFormatter(format);
            }
            catch (FormatterNotFoundException e)
            {
                _logger.LogWarning(e, $"Requested report formatter for '{format}' cannot be found.");
                return BadRequest($"Requested report formatter for '{format}' cannot be found.");
            }
            ReportData report = await _reportService.GetReportByStudent(studentDto, courseDto);
            string reportString = formatter.FormatReport(report);
            return reportString;
        }

        [HttpGet("lecture.{format}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> GetReportByLecture(string format, int lectureId)
        {
            if (lectureId <= 0)
            {
                ModelState.AddModelError(nameof(lectureId), "'lectureId' must be positive");
                return BadRequest(ModelState);
            }
            
            LectureDto? lectureDto = await _lectureService.GetLecture(lectureId);
            if (lectureDto is null) return BadRequest($"Lecture with id {lectureId} does not exist.");

            IReportFormatter formatter;
            try
            {
                formatter = _formatterManager.GetFormatter(format);
            }
            catch (FormatterNotFoundException e)
            {
                _logger.LogWarning(e, $"Requested report formatter for '{format}' cannot be found.");
                return BadRequest($"Requested report formatter for '{format}' cannot be found.");
            }
            ReportData report = await _reportService.GetReportByLecture(lectureDto);
            string reportString = formatter.FormatReport(report);
            return reportString;
        }
    }
}
