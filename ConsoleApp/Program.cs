using Application;
using Application.Repos;
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
				var repo = GetInputFromConsole();

				try
				{
					var commitService = services.GetRequiredService<RepoService>();
					var commits = commitService.GetCommits(repo, owner);

					foreach (var commit in commits)
					{
						Console.WriteLine($"{owner}/{repo}/{commit.Sha}: {commit.Message} [{commit.Committer}]");
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
