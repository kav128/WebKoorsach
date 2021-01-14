using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EduJournal.BLL.Services.Report.Formats
{
    public class JsonReportFormatter : IReportFormatter
    {
        public string FormatReport(ReportData reportData)
        {
            var serializerOptions = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                WriteIndented = true
            };
            string jsonString = JsonSerializer.Serialize(reportData, typeof(ReportData), serializerOptions);
            return jsonString;
        }
    }
}
