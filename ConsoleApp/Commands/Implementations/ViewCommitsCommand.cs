using ConsoleApp.UI;
using Domain.Repositories;

namespace ConsoleApp.Commands.Implementations
{
	public class ViewCommitsCommand : ICommand
	{
		private readonly IUserInterface _ui;
		private readonly IRepoRepository _repoRepository;

		public string Name => "view";
		public string Description => "View commits for a repository (reading from DB)";

		public ViewCommitsCommand(IUserInterface ui, IRepoRepository repoRepository)
		{
			_ui = ui;
			_repoRepository = repoRepository;
		}

		public async Task ExecuteAsync()
		{
			var owner = _ui.GetInput("Enter repository owner:");
			var repoName = _ui.GetInput("Enter repository name:");

			var repo = await _repoRepository.TryGetRepoAsync(repoName, owner);
			if (repo == null || !repo.Commits.Any())
			{
				_ui.DisplayMessage("No commits found.");
				return;
			}

			foreach (var commit in repo.Commits)
			{
				_ui.DisplayCommit(repoName, commit.Sha, commit.Message, commit.Committer);
			}
		}
	}
}
