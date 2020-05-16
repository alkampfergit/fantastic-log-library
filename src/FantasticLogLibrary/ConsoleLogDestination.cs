using System;

namespace FantasticLogLibrary
{
	public class ConsoleLogDestination : ILogDestination
	{
		public void AddLog(LogMessage message)
		{
			Console.WriteLine(message.RenderedMessage);
		}
	}
}
