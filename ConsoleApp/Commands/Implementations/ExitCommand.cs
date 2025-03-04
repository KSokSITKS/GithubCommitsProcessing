namespace ConsoleApp.Commands.Implementations
{
	public class ExitCommand : ICommand
	{
		public string Name => "exit";
		public string Description => "Exit the application";

		public Task ExecuteAsync()
		{
			Environment.Exit(0);
			return Task.CompletedTask;
		}
	}
}
