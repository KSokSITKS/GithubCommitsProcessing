namespace Domain
{
	public interface IUserInterface
	{
		string GetInput(string prompt);
		void DisplayCommit(string repository, string sha, string message, string committer);
		void DisplayError(string message);
		void DisplayMessage(string message);
	}
}
