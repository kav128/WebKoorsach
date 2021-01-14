using System.Xml;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;

namespace EduJournal.BLL.Services.Report.Formats
{
    public class XmlReportFormatter : IReportFormatter
    {
        public string FormatReport(ReportData reportData)
        {
            IExtendedXmlSerializer serializer = new ConfigurationContainer()
                .EnableImplicitTypingFromAll<ReportData>()
                .Type<ReportData>().Name("Report")
                .Type<ReportRecord>().Name("Record")
                .Create();
            var xmlWriterSettings = new XmlWriterSettings { Indent = true };
            string xmlString = serializer.Serialize(xmlWriterSettings, reportData);

            return xmlString;
        }
    }
}
