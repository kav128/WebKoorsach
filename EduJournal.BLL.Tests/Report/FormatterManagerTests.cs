using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EduJournal.BLL.Services.Report.Formats;
using Moq;
using NUnit.Framework;

namespace EduJournal.BLL.Tests.Report
{
    [TestFixture]
    public class FormatterManagerTests
    {
        public static IEnumerable<IEnumerable<object>> RegisterFormatterTestCaseSource()
        {
            yield return new object[] { "json", new JsonReportFormatter() };
            yield return new object[] { "xml", new XmlReportFormatter() };
        }

        [TestCaseSource(nameof(RegisterFormatterTestCaseSource))]
        public void RegisterFormatterTest(string name, IReportFormatter formatter)
        {
            // Arrange
            Mock<IDictionary<string, IReportFormatter>> dictionaryMock = new();
            dictionaryMock.Setup(formatters => formatters.Add(name, formatter)).Verifiable();
            FormatterManager manager = new(dictionaryMock.Object);

            // Act
            manager.RegisterFormatter(name, formatter);

            // Assert
            dictionaryMock.Verify();
        }

        public static IEnumerable<IEnumerable<object>> RegisterFormatterExceptionTestCaseGenerator()
        {
            yield return new object[]
            {
                new Dictionary<string, IReportFormatter>(),
                null,
                new JsonReportFormatter(),
                typeof(ArgumentNullException)
            };
            yield return new object[]
            {
                new Dictionary<string, IReportFormatter>(),
                "json",
                null,
                typeof(ArgumentNullException)
            };
            yield return new object[]
            {
                new ReadOnlyDictionary<string, IReportFormatter>(new Dictionary<string, IReportFormatter>()),
                "json",
                new JsonReportFormatter(),
                typeof(RegistrationNotSupportedException)
            };
            yield return new object[]
            {
                new Dictionary<string, IReportFormatter>
                {
                    { "json", new JsonReportFormatter() }
                },
                "json",
                new JsonReportFormatter(),
                typeof(DuplicatedFormatterException)
            };
        }

        [TestCaseSource(nameof(RegisterFormatterExceptionTestCaseGenerator))]
        public void RegisterFormatterTest_ThrowsException(IDictionary<string, IReportFormatter> storage,
            string name,
            IReportFormatter formatter,
            Type expectedException)
        {
            // Arrange
            var manager = new FormatterManager(storage);

            // Act
            void TestAction() => manager.RegisterFormatter(name, formatter);

            // Assert
            Assert.That(TestAction, Throws.TypeOf(expectedException));
        }

        #region GetFormatter

        public static IEnumerable<IEnumerable<object>> GetFormatterTestCaseGenerator()
        {
            IReportFormatter jsonFormatter = new JsonReportFormatter();
            IReportFormatter xmlFormatter = new XmlReportFormatter();

            yield return new object[]
            {
                new Dictionary<string, IReportFormatter> { { "json", jsonFormatter } },
                "json",
                jsonFormatter
            };
            yield return new object[]
            {
                new Dictionary<string, IReportFormatter> { { "json", jsonFormatter }, { "xml", xmlFormatter } },
                "json",
                jsonFormatter
            };
            yield return new object[]
            {
                new Dictionary<string, IReportFormatter> { { "json", jsonFormatter }, { "xml", xmlFormatter } },
                "xml",
                xmlFormatter
            };
        }

        [TestCaseSource(nameof(GetFormatterTestCaseGenerator))]
        public void GetFormatterTest(IDictionary<string, IReportFormatter> formatterStorage, string formatterName, IReportFormatter expected)
        {
            // Arrange
            var manager = new FormatterManager(formatterStorage);

            // Act
            IReportFormatter formatter = manager.GetFormatter(formatterName);

            // Assert
            Assert.That(formatter, Is.EqualTo(expected));
        }

        public static IEnumerable<IEnumerable<object>> GetFormatterExceptionTestCaseGenerator()
        {
            yield return new object[]
            {
                "json"
            };
        }

        [TestCaseSource(nameof(GetFormatterExceptionTestCaseGenerator))]
        public void GetFormatterTest_ThrowsException(string formatterName)
        {
            // Arrange
            var manager = new FormatterManager();

            // Act
            IReportFormatter TestAction() => manager.GetFormatter(formatterName);

            // Assert
            Assert.That(TestAction, Throws.TypeOf<FormatterNotFoundException>());
        }

        #endregion
    }
}
