using Application.Repos;
using ConsoleApp.Commands;
using ConsoleApp.Commands.Implementations;
using ConsoleApp.UI;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConsoleAppUnitTests
{
	[TestClass]
	public class CommandFactoryTests
	{
		private IServiceProvider _serviceProvider;
		private CommandFactory _sut;

		[TestInitialize]
		public void TestInitialize()
		{
			var services = new ServiceCollection();

			// Mock required dependencies for commands
			services.AddScoped(_ => Mock.Of<IUserInterface>());
			services.AddScoped(_ => Mock.Of<IRepoService>());
			services.AddScoped(_ => Mock.Of<IRepoRepository>());

			_serviceProvider = services.BuildServiceProvider();
			_sut = new CommandFactory(_serviceProvider);
		}

		[TestMethod]
		public void Should_CreateLoadCommand_When_LoadCommandNameProvided()
		{
			// Arrange
			var commandName = "load";

			// Act
			var result = _sut.CreateCommand(commandName);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(LoadCommitsFromGithubCommand));
		}

		[TestMethod]
		public void Should_CreateViewCommand_When_ViewCommandNameProvided()
		{
			// Arrange
			var commandName = "view";

			// Act
			var result = _sut.CreateCommand(commandName);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(ViewCommitsCommand));
		}

		[TestMethod]
		public void Should_CreateExitCommand_When_ExitCommandNameProvided()
		{
			// Arrange
			var commandName = "exit";

			// Act
			var result = _sut.CreateCommand(commandName);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(ExitCommand));
		}

		[TestMethod]
		public void Should_ReturnNull_When_InvalidCommandNameProvided()
		{
			// Arrange
			var invalidCommandName = "invalid";

			// Act
			var result = _sut.CreateCommand(invalidCommandName);

			// Assert
			Assert.IsNull(result);
		}

		[TestMethod]
		public void Should_HandleCaseInsensitiveCommands_When_DifferentCaseProvided()
		{
			// Arrange
			var upperCaseCommand = "LOAD";
			var mixedCaseCommand = "ViEw";

			// Act
			var upperCaseResult = _sut.CreateCommand(upperCaseCommand);
			var mixedCaseResult = _sut.CreateCommand(mixedCaseCommand);

			// Assert
			Assert.IsInstanceOfType(upperCaseResult, typeof(LoadCommitsFromGithubCommand));
			Assert.IsInstanceOfType(mixedCaseResult, typeof(ViewCommitsCommand));
		}

		[TestMethod]
		public void Should_ReturnAllCommands_When_GettingAvailableCommands()
		{
			// Act
			var availableCommands = _sut.GetAvailableCommands().ToList();

			// Assert
			Assert.AreEqual(3, availableCommands.Count);
			Assert.IsTrue(availableCommands.Any(c => c is LoadCommitsFromGithubCommand));
			Assert.IsTrue(availableCommands.Any(c => c is ViewCommitsCommand));
			Assert.IsTrue(availableCommands.Any(c => c is ExitCommand));
		}

		[TestMethod]
		public void Should_CreateNewInstances_When_RequestingSameCommandMultipleTimes()
		{
			// Act
			var firstInstance = _sut.CreateCommand("load");
			var secondInstance = _sut.CreateCommand("load");

			// Assert
			Assert.IsNotNull(firstInstance);
			Assert.IsNotNull(secondInstance);
			Assert.AreNotSame(firstInstance, secondInstance);
		}
	}
}
