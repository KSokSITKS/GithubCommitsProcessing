using Domain.Entities;
using Domain.Repositories;
using Persistence.Database;

namespace Persistence.Repositories
{
	internal class RepoRepository : IRepoRepository
	{
		private readonly ApplicationDbContext _context;

		public RepoRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public void Add(Repo repo)
		{
			_context.Repos.Add(repo);
		}

		public Repo? TryGetRepo(string repositoryName, string repositoryOwner)
		{
			return _context.Repos.FirstOrDefault(r => r.Name == repositoryName && r.Owner == repositoryOwner);
		}
	}
}