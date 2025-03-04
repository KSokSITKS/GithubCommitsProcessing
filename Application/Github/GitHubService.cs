using System.Text.Json;

namespace Application.Github
{
	public class GitHubService : IGitHubService
	{
		private const string GitHubApiBaseUrl = "https://api.github.com";

		public List<GitHubCommitDto> GetCommitsFromRepository(string owner, string repositoryName)
		{
			using var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("User-Agent", "CommitService");
			httpClient.DefaultRequestHeaders.Accept.Add(new("application/vnd.github.v3+json"));

			try
			{
				var url = $"{GitHubApiBaseUrl}/repos/{owner}/{repositoryName}/commits";
				var response = httpClient.GetAsync(url).Result;
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
