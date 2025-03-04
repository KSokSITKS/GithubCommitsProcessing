using Application.Repos;
using Domain;
using Domain.Entities;
using Domain.Repositories;

namespace ConsoleApp.UI
{
	public class CommitViewer
	{
		private readonly IUserInterface _ui;
		private readonly RepoService _repoService;
		private readonly IRepoRepository _repoRepository;

		public CommitViewer(
			IUserInterface ui,
			RepoService repoService,
			IRepoRepository repoRepository)
		{
			_ui = ui;
			_repoService = repoService;
			_repoRepository = repoRepository;
		}

		public void Run()
		{
			while (true)
			{
				try
				{
					ProcessSingleRepository();
				}
				catch (Exception ex)
				{
					_ui.DisplayError(ex.Message);
				}
			}
		}

		private void ProcessSingleRepository()
		{
			var owner = _ui.GetInput("Please enter repository owner:");
			var repositoryName = _ui.GetInput("Please enter repository name:");

			_repoService.LoadCommitsToDb(repositoryName, owner);

			var repo = _repoRepository.TryGetRepo(repositoryName, owner);
			if (repo == null || !repo.Commits.Any())
			{
				_ui.DisplayMessage("No commits found.");
				return;
			}

			DisplayCommits(repo.Commits, repositoryName);
		}

		private void DisplayCommits(IEnumerable<Commit> commits, string repositoryName)
		{
			foreach (var commit in commits)
			{
				_ui.DisplayCommit(
					repositoryName,
					commit.Sha,
					commit.Message,
					commit.Committer);
			}
		}
	}
}
