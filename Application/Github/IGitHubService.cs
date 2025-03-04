namespace Application.Github
{
	public interface IGitHubService
	{
		List<GitHubCommitDto> GetCommitsFromRepository(string owner, string repositoryName);
		Task<List<GitHubCommitDto>> GetCommitsFromRepositoryAsync(string owner, string repositoryName);
	}
}
