using System;
using System.IO;

namespace FantasticLogLibrary.FileDestination
{
    public class SingleFileLogDestination : ILogDestination
    {
        private FileStream _fileStream;
        private StreamWriter _streamWriter;

        public SingleFileLogDestination(String fileName)
        {
            _fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            _fileStream.Seek(0, SeekOrigin.End);
            _streamWriter = new StreamWriter(_fileStream);
        }

        public void AddLog(LogMessage message)
        {
            _streamWriter.Write($"{message.Level}[{message.Timestamp.ToString("s")}] - {message.RenderedMessage}\n");
        }

        public void Flush()
        {
            _streamWriter.Flush();
        }
    }
}
