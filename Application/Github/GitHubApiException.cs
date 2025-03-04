using Domain.Exceptions;

namespace Application.Github
{
	public class GitHubApiException : ProgramGeneralException
	{
		public GitHubApiException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

}
