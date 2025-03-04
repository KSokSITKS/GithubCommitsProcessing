using Domain.Entities;
using Domain.Repositories;
using Persistence.Database;

namespace Persistence.Repositories
{
	internal class CommitRepository : ICommitRepository
	{
		private readonly ApplicationDbContext _context;

		public CommitRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public void AddMany(IEnumerable<Commit> commits)
		{
			_context.Commits.AddRange(commits);
		}

		public List<Commit> GetCommitsForRepository(Guid repositoryId)
		{
			return _context.Commits
				.Where(c => c.RepositoryId == repositoryId)
				.ToList();
		}
	}
}