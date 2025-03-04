using Application;
using Application.Github;
using Application.Repos;
using ConsoleApp.UI;
using Domain;
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
			var viewer = services.GetRequiredService<GitHubCommitViewer>();
			
			viewer.Run();
		}

		private static ServiceProvider RegisterServices()
		{
			var serviceProvider = new ServiceCollection()
				// services
				.AddApplication()
				.AddPersistence()
				.AddSingleton<IUserInterface, ConsoleUI>()
				.BuildServiceProvider();

			return serviceProvider;
		}
	}
}
