using Application.Repos;
using ConsoleApp.Commands;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;

namespace ConsoleApp.UI
{
	public class CommitViewer
	{
		private readonly IUserInterface _ui;
		private readonly ICommandFactory _commandFactory;

		public CommitViewer(IUserInterface ui, ICommandFactory commandFactory)
		{
			_ui = ui;
			_commandFactory = commandFactory;
		}

		public async Task RunAsync()
		{
			while (true)
			{
				DisplayAvailableCommands();
				var commandName = _ui.GetInput("Enter command:");

				var command = _commandFactory.CreateCommand(commandName);
				if (command == null)
				{
					_ui.DisplayError("Invalid command");
					continue;
				}

				try
				{
					await command.ExecuteAsync();
				}
				catch (ProgramGeneralException ex)
				{
					_ui.DisplayError(ex.Message);
				}
				catch (Exception ex)
				{
					_ui.DisplayError($"Unhandled exception: {ex.Message}");
				}
			}
		}

		private void DisplayAvailableCommands()
		{
			_ui.DisplayMessage("\nAvailable commands:");
			foreach (var command in _commandFactory.GetAvailableCommands())
			{
				_ui.DisplayMessage($"  {command.Name} - {command.Description}");
			}
		}
	}
}
