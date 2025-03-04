using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICommitRepository
    {
        void AddMany(IEnumerable<Commit> commits);
        List<Commit> GetCommitsForRepository(Guid repositoryId);
	}
}
