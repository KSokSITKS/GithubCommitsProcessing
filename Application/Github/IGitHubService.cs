namespace Application.Github
{
	public interface IGitHubService
	{
		Task<List<GitHubCommitDto>> GetCommitsFromRepositoryAsync(string owner, string repositoryName);
	}
}
