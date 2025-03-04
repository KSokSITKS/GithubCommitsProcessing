using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp.Commands.Implementations
{
	public class CommandFactory : ICommandFactory
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly Dictionary<string, Type> _commands;

		public CommandFactory(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
			_commands = new Dictionary<string, Type>
			{
				{ "load", typeof(LoadCommitsFromGithubCommand) },
				{ "view", typeof(ViewCommitsCommand) },
				{ "exit", typeof(ExitCommand) }
			};
		}

		public ICommand? CreateCommand(string commandName)
		{
			if (!_commands.TryGetValue(commandName.ToLower(), out var commandType))
				return null;

			return (ICommand)ActivatorUtilities.CreateInstance(_serviceProvider, commandType);
		}

		public IEnumerable<ICommand> GetAvailableCommands()
		{
			return _commands.Values
				.Select(type => (ICommand)ActivatorUtilities.CreateInstance(_serviceProvider, type));
		}
	}
}
