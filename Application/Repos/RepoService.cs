using Application.Github;
using Domain;
using Domain.Entities;
using Domain.Repositories;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Repos
{
    public sealed class RepoService
    {
        private readonly ICommitRepository _commitsRepository;
		private readonly IRepoRepository _repoRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IGitHubService _gitHubService;
		private const string GitHubApiBaseUrl = "https://api.github.com";

		public RepoService(ICommitRepository commitsRepository, IRepoRepository repoRepository, IUnitOfWork unitOfWork, IGitHubService gitHubService)
		{
			_commitsRepository = commitsRepository;
			_repoRepository = repoRepository;
			_unitOfWork = unitOfWork;
			_gitHubService = gitHubService;
		}

		public void LoadCommitsToDb(string repositoryName, string repositoryOwner)
		{
			if (string.IsNullOrWhiteSpace(repositoryOwner))
				throw new ArgumentException("Repository owner cannot be empty");

			if (string.IsNullOrWhiteSpace(repositoryName))
				throw new ArgumentException("Repository name cannot be empty");

			try
			{
				var githubCommits = _gitHubService.GetCommitsFromRepository(repositoryOwner, repositoryName);

				if (githubCommits == null)
					return;

				var repoId = SaveRepo(repositoryName, repositoryOwner);

				var commits = githubCommits.Select(gc => new Commit
				{
					Sha = gc.Sha,
					Message = gc.Commit.Message,
					Committer = gc.Commit.Committer.Name,
					RepositoryId = repoId
				}).ToList();
				
				SaveCommits(commits, repoId);

				_unitOfWork.SaveChanges();
			}
			catch (HttpRequestException ex)
			{
				throw new Exception($"Error accessing GitHub API: {ex.Message}", ex);
			}
		}

		private void SaveCommits(List<Commit> commits, Guid repoId)
		{
			if (commits.Any())
			{
				var existingCommitShas = _commitsRepository
					.GetCommitsForRepository(repoId)
					.Select(c => c.Sha)
					.ToHashSet();

				var commitsToAdd = commits
					.Where(c => !existingCommitShas.Contains(c.Sha))
					.ToList();

				Console.WriteLine($"Added {commitsToAdd.Count()} ");

				_commitsRepository.AddMany(commitsToAdd);
			}
		}

		private Guid SaveRepo(string repositoryName, string repositoryOwner)
		{
			var repo = _repoRepository.TryGetRepo(repositoryName, repositoryOwner);

			if (repo != null)
			{
				return repo.Id;
			}
			else
			{
				var newRepo = new Repo
				{
					Name = repositoryName,
					Owner = repositoryOwner
				};

				_repoRepository.Add(newRepo);

				return newRepo.Id;
			}
		}

		private class GitHubCommit
		{
			[JsonPropertyName("sha")]
			public string Sha { get; set; } = string.Empty;

			[JsonPropertyName("commit")]
			public CommitDetail Commit { get; set; } = new();

			public class CommitDetail
			{
				[JsonPropertyName("message")]
				public string Message { get; set; } = string.Empty;

				[JsonPropertyName("committer")]
				public Committer Committer { get; set; } = new();
			}

			public class Committer
			{
				[JsonPropertyName("name")]
				public string Name { get; set; } = string.Empty;
			}
		}
	}
}
