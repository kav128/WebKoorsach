using System.Collections.Generic;
using AutoMapper;
using EduJournal.BLL.Services.Entity;
using EduJournal.BLL.Services.Report;
using EduJournal.BLL.Services.Report.Formats;
using Microsoft.Extensions.DependencyInjection;

namespace EduJournal.BLL
{
    public static class BusinessLogicLayerDI
    {
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
        {
            return services
                .AddFormatterManager()
                .AddTransient<IJournalService, JournalService>()
                .AddTransient<ILecturerService, LecturerService>()
                .AddTransient<IStudentService, StudentService>()
                .AddTransient<ICourseService, CourseService>()
                .AddTransient<ILectureService, LectureService>()
                .AddTransient<IReportService, ReportService>()
                .AddTransient<IMessageSenderFactory, MessageSenderFactory>();
        }

        private static IServiceCollection AddFormatterManager(this IServiceCollection services)
        {
            IFormatterManager formatterManager = new FormatterManager();
            formatterManager.RegisterFormatter("json", new JsonReportFormatter());
            formatterManager.RegisterFormatter("xml", new XmlReportFormatter());
            return services.AddSingleton(formatterManager);
        }

        public static IEnumerable<Profile> GetAutoMapperProfiles()
        {
            yield return new DtoProfile();
        }
    }
}
