namespace FantasticLogLibrary
{
	public interface ILogDestination
	{
		void AddLog(LogMessage message);
	}
}
