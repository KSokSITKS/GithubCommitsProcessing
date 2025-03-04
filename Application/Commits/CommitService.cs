using Domain.Entities;
using Domain.Repositories;

namespace Application.Commits
{
	public class CommitService : ICommitService
	{
		private readonly ICommitRepository _commitRepository;

		public CommitService(ICommitRepository commitRepository)
		{
			_commitRepository = commitRepository;
		}

		public void SaveNewCommits(List<Commit> commits, Guid repoId)
		{
			if (!commits.Any()) 
				return;

			var existingCommitShas = _commitRepository
				.GetCommitsForRepository(repoId)
				.Select(c => c.Sha)
				.ToHashSet();

			var commitsToAdd = commits
				.Where(c => !existingCommitShas.Contains(c.Sha))
				.ToList();

			if (commitsToAdd.Any())
			{
				_commitRepository.AddMany(commitsToAdd);
			}
		}
	}
}
