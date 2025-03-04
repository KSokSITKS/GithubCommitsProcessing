using Domain.Entities;

namespace Application.Commits
{
	public interface ICommitService
	{
		void SaveNewCommits(List<Commit> commits, Guid repoId);
	}
}
