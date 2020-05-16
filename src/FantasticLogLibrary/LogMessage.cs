using System;

namespace FantasticLogLibrary
{
	public class LogMessage
	{
        public LogMessage(LogLevel level, string renderedMessage, DateTime timestamp)
        {
            Level = level;
            RenderedMessage = renderedMessage;
            Timestamp = timestamp;
        }

        public LogLevel Level { get; set; }

		public string RenderedMessage { get; set; }

		public DateTime Timestamp { get; set; }
	}
}