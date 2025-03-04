
namespace Application.Repos
{
	public interface IRepoService
	{
		Task LoadCommitsToDbAsync(string repositoryName, string repositoryOwner);
	}
}