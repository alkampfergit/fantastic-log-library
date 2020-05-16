using FantasticLogLibrary.FileDestination;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FantasticLogLibrary.Tests.FileDestination
{
    [TestFixture]
    public class SingleFileLogDestinationTests
    {
        private SingleFileLogDestination _sut;
        private string _fileName;
        private readonly DateTime _aDate = new DateTime(2010, 10, 10, 11, 12, 13);

        [SetUp]
        public void SetUp()
        {
            _fileName = Path.GetTempFileName();
            _sut = new SingleFileLogDestination(_fileName);
        }

        [Test]
        public void Write_simple_log_on_file_contains_single_line_with_message()
        {
            _sut.AddLog(new LogMessage(LogLevel.Debug, "This is a message", _aDate));
            _sut.Flush();
            var fileContent = ReadAllLinesOfLogFile();
            Assert.That(fileContent.Count, Is.EqualTo(1));
            Assert.That(fileContent[0].Contains("This is a message"));
        }

        [Test]
        public void Write_simple_log_on_file_contains_date()
        {
            _sut.AddLog(new LogMessage(LogLevel.Debug, "This is a message", _aDate));
            _sut.Flush();
            var fileContent = ReadAllLinesOfLogFile();
            Assert.That(fileContent.Count, Is.EqualTo(1));
            Assert.That(fileContent[0].Contains(_aDate.ToString("s")));
        }

        [Test]
        public void Write_simple_log_on_file_contains_level()
        {
            _sut.AddLog(new LogMessage(LogLevel.Warn, "This is a message", _aDate));
            _sut.Flush();
            var fileContent = ReadAllLinesOfLogFile();
            Assert.That(fileContent.Count, Is.EqualTo(1));
            Assert.That(fileContent[0].Contains("Warn"));
        }

        private List<string> ReadAllLinesOfLogFile()
        {
            List<String> lines = new List<string>();
            var fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var reader = new StreamReader(fs))
            {
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }
            }
            return lines;
        }
    }
}
