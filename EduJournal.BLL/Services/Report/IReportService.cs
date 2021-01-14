using System.Threading.Tasks;
using EduJournal.BLL.DTO;

namespace EduJournal.BLL.Services.Report
{
    public interface IReportService
    {
        public Task<ReportData> GetReportByLecture(LectureDto lecture);

        public Task<ReportData> GetReportByStudent(StudentDto student, CourseDto course);
    }
}