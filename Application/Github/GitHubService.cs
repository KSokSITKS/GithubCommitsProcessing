using System.Text.Json;

namespace Application.Github
{
	public class GitHubService : IGitHubService
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public GitHubService(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public List<GitHubCommitDto> GetCommitsFromRepository(string owner, string repositoryName)
		{
			var client = _httpClientFactory.CreateClient(GitHubClientConfig.Name);

			try
			{
				var url = $"repos/{owner}/{repositoryName}/commits";
				var response = client.GetAsync(url).Result;
				response.EnsureSuccessStatusCode();

				var content = response.Content.ReadAsStringAsync();
				return JsonSerializer.Deserialize<List<GitHubCommitDto>>(content.Result) ?? new();
			}
			catch (HttpRequestException ex)
			{
				throw new GitHubApiException($"Error accessing GitHub API: {ex.Message}", ex);
			}
		}
	}
}
