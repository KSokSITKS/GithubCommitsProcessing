using Domain;

namespace ConsoleApp.UI
{
	public class ConsoleUI : IUserInterface
	{
		public string GetInput(string prompt)
		{
			Console.WriteLine(prompt);
			return Console.ReadLine()?.Trim() ?? string.Empty;
		}

		public void DisplayCommit(string repository, string sha, string message, string committer)
		{
			Console.WriteLine($"{repository}/{sha}: {message} [{committer}]");
		}

		public void DisplayError(string message)
		{
			var currentColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"Error: {message}");
			Console.ForegroundColor = currentColor;
		}

		public void DisplayMessage(string message)
		{
			Console.WriteLine(message);
		}
	}
}
