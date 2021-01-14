using System.Collections.Generic;
using System.Xml.Linq;
using EduJournal.BLL.Services.Report;
using EduJournal.BLL.Services.Report.Formats;
using NUnit.Framework;

namespace EduJournal.BLL.Tests.Report
{
    [TestFixture]
    public class XmlFormatterTests
    {
        public static IEnumerable<IEnumerable<object>> FormatReportTestCaseGenerator()
        {
            yield return new object[]
            {
                new ReportData
                {
                    Header = new ReportHeader
                    {
                        Student = "Isaac Newton",
                        Course = "Physics"
                    },
                    Records = new ReportRecord[]
                    {
                        new() { Lecture = "Physics, Lecture 1", Attendance = true, Score = 5 },
                        new() { Lecture = "Physics, Lecture 2", Attendance = true, Score = 4 },
                        new() { Lecture = "Physics, Lecture 3", Attendance = false, Score = 0 }
                    },
                    AverageScore = 3.0,
                    AttendancePercentage = 0.67
                },
                new XElement("Report",
                    new XElement("Header",
                        new XElement("Student", "Isaac Newton"),
                        new XElement("Course", "Physics")),
                    new XElement("Records",
                        new XElement("Record",
                            new XElement("Lecture", "Physics, Lecture 1"),
                            new XElement("Attendance", true),
                            new XElement("Score", 5)),
                        new XElement("Record",
                            new XElement("Lecture", "Physics, Lecture 2"),
                            new XElement("Attendance", true),
                            new XElement("Score", 4)),
                        new XElement("Record",
                            new XElement("Lecture", "Physics, Lecture 3"),
                            new XElement("Attendance", false),
                            new XElement("Score", 0))
                    ),
                    new XElement("AverageScore", 3.0),
                    new XElement("AttendancePercentage", 0.67))
            };

            yield return new object[]
            {
                new ReportData
                {
                    Header = new ReportHeader
                    {
                        Course = "Physics",
                        Lecture = "Physics, Lecture 1"
                    },
                    Records = new ReportRecord[]
                    {
                        new() { Student = "Isaac Newton", Attendance = true, Score = 5 },
                        new() { Student = "Albert Einstein", Attendance = true, Score = 4 },
                        new() { Student = "Nikola Tesla", Attendance = false, Score = 0 }
                    }
                },
                new XElement("Report",
                    new XElement("Header",
                        new XElement("Lecture", "Physics, Lecture 1"),
                        new XElement("Course", "Physics")),
                    new XElement("Records",
                        new XElement("Record",
                            new XElement("Student", "Isaac Newton"),
                            new XElement("Attendance", true),
                            new XElement("Score", 5)),
                        new XElement("Record",
                            new XElement("Student", "Albert Einstein"),
                            new XElement("Attendance", true),
                            new XElement("Score", 4)),
                        new XElement("Record",
                            new XElement("Student", "Nikola Tesla"),
                            new XElement("Attendance", false),
                            new XElement("Score", 0))
                    ))
            };
        }
        
        [TestCaseSource(nameof(FormatReportTestCaseGenerator))]
        public void FormatReportTest(ReportData data, XElement expected)
        {
            // Arrange
            var formatter = new XmlReportFormatter();
            string xmlString = formatter.FormatReport(data);
            
            // Act
            XDocument document = XDocument.Parse(xmlString);
            
            // Assert
            Assert.That(XNode.DeepEquals(document.Root, expected), "Elements are not equal");
        }
    }
}
