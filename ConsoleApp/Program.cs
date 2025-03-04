using Application;
using ConsoleApp.Commands.Implementations;
using ConsoleApp.Commands;
using ConsoleApp.UI;
using Microsoft.Extensions.DependencyInjection;
using Persistence;

namespace ConsoleApp
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			var services = RegisterServices();
			var viewer = services.GetRequiredService<CommitViewer>();
			
			await viewer.RunAsync();
		}

		private static ServiceProvider RegisterServices()
		{
			var serviceProvider = new ServiceCollection()
				// services
				.AddApplication()
				.AddPersistence()
				.AddSingleton<IUserInterface, ConsoleUI>()
				.AddSingleton<CommitViewer>()
				.AddSingleton<ICommandFactory, CommandFactory>()
				.BuildServiceProvider();

			return serviceProvider;
		}
	}
}
