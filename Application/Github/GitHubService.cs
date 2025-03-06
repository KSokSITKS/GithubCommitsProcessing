using System.Net.Http.Headers;
using System.Text.Json;

namespace Application.Github
{
	public class GitHubService : IGitHubService
	{
		private const string LinkHeaderName = "Link";
		private const string NextRelation = "rel=\"next\"";
		private const int RecordsPerPage = 100;

		private readonly IHttpClientFactory _httpClientFactory;

		public GitHubService(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task<List<GitHubCommitDto>> GetCommitsFromRepositoryAsync(string owner, string repositoryName)
		{
			var allCommits = new List<GitHubCommitDto>();
			var client = _httpClientFactory.CreateClient(GitHubClientConfig.Name);
			var currentUrl = $"repos/{owner}/{repositoryName}/commits?per_page={RecordsPerPage}";

			try
			{
				while (!string.IsNullOrEmpty(currentUrl))
				{
					var response = await client.GetAsync(currentUrl);
					response.EnsureSuccessStatusCode();

					var content = await response.Content.ReadAsStringAsync();
					var pageCommits = JsonSerializer.Deserialize<List<GitHubCommitDto>>(content) ?? new();
					allCommits.AddRange(pageCommits);

					currentUrl = GetNextPageUrl(response.Headers);
				}

				return allCommits;
			}
			catch (HttpRequestException ex)
			{
				throw new GitHubApiException($"Error accessing GitHub API: {ex.Message}", ex);
			}
		}

		private string? GetNextPageUrl(HttpResponseHeaders headers)
		{
			var isPaginatedResponse = !headers.Contains(LinkHeaderName);
			if (isPaginatedResponse) 
				return null;

			var links = headers.GetValues(LinkHeaderName).FirstOrDefault();
			if (string.IsNullOrEmpty(links)) 
				return null;

			var hasNextPage = links.Contains(NextRelation);

			if (hasNextPage)
			{
				var allRels = links.Split(',');
				var nextPageRel = allRels.Single(s => s.Contains(NextRelation));
				var linkStartIndex = nextPageRel.IndexOf('<');
				var linkEndIndex = nextPageRel.IndexOf('>');

				return nextPageRel.Substring(linkStartIndex + 1, linkEndIndex - linkStartIndex - 1);
			}

			return null;
		}
	}
}
