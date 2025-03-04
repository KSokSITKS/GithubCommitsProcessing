using Domain.Entities;

namespace Domain.Repositories
{
    public interface IRepoRepository
    {
        void Add(Repo repo);
		Repo? TryGetRepo(string repositoryName, string repositoryOwner);
	}
}
