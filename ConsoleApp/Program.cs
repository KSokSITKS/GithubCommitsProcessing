using Application;
using Application.Repos;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Persistence;

namespace ConsoleApp
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var services = RegisterServices();

			while (true)
			{
				Console.WriteLine("Please enter repository owner:");
				var owner = GetInputFromConsole();

				Console.WriteLine("Please enter repository name:");
				var repositoryName = GetInputFromConsole();

				try
				{
					var commitService = services.GetRequiredService<RepoService>();
					commitService.LoadCommitsToDb(repositoryName, owner);

					var repoRepository = services.GetRequiredService<IRepoRepository>();

					var repoObj = repoRepository.TryGetRepo(repositoryName, owner);
					var commits = repoObj != null
						? repoObj.Commits
						: new List<Commit>();

					foreach (var commit in commits)
					{
						Console.WriteLine($"{repositoryName}/{commit.Sha}: {commit.Message} [{commit.Committer}]");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error: {ex.Message}");
				}
			}
		}

		private static string GetInputFromConsole()
		{
			return Console.ReadLine()?.Trim() ?? string.Empty;
		}

		private static ServiceProvider RegisterServices()
		{
			var serviceProvider = new ServiceCollection()
				// services
				.AddApplication()
				.AddPersistence()
				.BuildServiceProvider();

			return serviceProvider;
		}
	}
}
