namespace ConsoleApp.Commands
{
	public interface ICommandFactory
	{
		ICommand? CreateCommand(string commandName);
		IEnumerable<ICommand> GetAvailableCommands();
	}
}
