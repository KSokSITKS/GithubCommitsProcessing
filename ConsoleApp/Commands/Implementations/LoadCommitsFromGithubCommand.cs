using Application.Repos;
using ConsoleApp.UI;

namespace ConsoleApp.Commands.Implementations
{
	public class LoadCommitsFromGithubCommand : ICommand
	{
		private readonly IUserInterface _userInterface;
		private readonly RepoService _repoService;

		public string Name => "load";
		public string Description => "Load commits from GitHub repository (saving to DB)";

		public LoadCommitsFromGithubCommand(RepoService repoService, IUserInterface userInterface)
		{
			_repoService = repoService;
			_userInterface = userInterface;
		}

		public async Task ExecuteAsync()
		{
			var owner = _userInterface.GetInput("Enter repository owner:");
			var repoName = _userInterface.GetInput("Enter repository name:");

			_userInterface.DisplayMessage("Loading commits from GitHub...");
			await _repoService.LoadCommitsToDbAsync(repoName, owner);
			_userInterface.DisplayMessage("Commits loaded successfully");
		}
	}
}
