using System.Collections.Generic;
using System.Text.Json;
using EduJournal.BLL.Services.Report;
using EduJournal.BLL.Services.Report.Formats;
using NUnit.Framework;

namespace EduJournal.BLL.Tests.Report
{
    [TestFixture]
    public class JsonReportFormatterTests
    {
        public static IEnumerable<IEnumerable<object>> FormatReportTestsCaseSource()
        {
            yield return new object[]
            {
                new ReportData
                {
                    Header = new ReportHeader { Student = "Isaac Newton", Course = "Physics", Lecture = null },
                    Records = new ReportRecord[]
                    {
                        new() { Lecture = "Physics, Lecture 1", Attendance = true, Score = 5, Student = null },
                        new() { Lecture = "Physics, Lecture 2", Attendance = true, Score = 4, Student = null },
                        new() { Lecture = "Physics, Lecture 3", Attendance = false, Score = 0, Student = null }
                    },
                    AverageScore = 3.0,
                    AttendancePercentage = 0.67
                }
            };
            
            yield return new object[]
            {
                new ReportData
                {
                    Header = new ReportHeader { Course = "Physics", Lecture = "Physics, Lecture 1", Student = null },
                    Records = new ReportRecord[]
                    {
                        new() { Student = "Isaac Newton", Attendance = true, Score = 5, Lecture = null },
                        new() { Student = "Albert Einstein", Attendance = true, Score = 4, Lecture = null },
                        new() { Student = "Nikola Tesla", Attendance = false, Score = 0, Lecture = null }
                    }
                }
            };
        }
        
        [TestCaseSource(nameof(FormatReportTestsCaseSource))]
        public static void FormatReportTests(ReportData reportData)
        {
            // Arrange
            JsonReportFormatter formatter = new();

            // Act
            string json = formatter.FormatReport(reportData);

            // Assert
            var actual = JsonSerializer.Deserialize<ReportData>(json);
            Assert.That(actual, Is.EqualTo(reportData));
        }
    }
}
