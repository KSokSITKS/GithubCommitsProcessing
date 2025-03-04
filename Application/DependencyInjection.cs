using Application.Commits;
using Application.Github;
using Application.Repos;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			services.AddScoped<RepoService>();
			services.AddSingleton<IGitHubService, GitHubService>();
			services.AddSingleton<ICommitService, CommitService>();

			return services;
		}
	}
}
