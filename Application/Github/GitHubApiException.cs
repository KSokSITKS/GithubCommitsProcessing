namespace Application.Github
{
	public class GitHubApiException : Exception
	{
		public GitHubApiException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

}
