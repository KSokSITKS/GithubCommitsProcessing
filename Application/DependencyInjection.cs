using Application.Commits;
using Application.Github;
using Application.Repos;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			services.AddSingleton<IRepoService, RepoService>();
			services.AddSingleton<IGitHubService, GitHubService>();
			services.AddSingleton<ICommitService, CommitService>();
			services.AddHttpClient(GitHubClientConfig.Name, client =>
			{
				client.BaseAddress = new Uri("https://api.github.com/");
				client.DefaultRequestHeaders.Add("User-Agent", "YourAppName");
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
			});

			return services;
		}
	}
}
