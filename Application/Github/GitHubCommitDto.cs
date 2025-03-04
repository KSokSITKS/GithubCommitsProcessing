using System.Text.Json.Serialization;

namespace Application.Github
{
	public class GitHubCommitDto
	{
		[JsonPropertyName("sha")]
		public string Sha { get; set; } = string.Empty;

		[JsonPropertyName("commit")]
		public CommitDetailDto Commit { get; set; } = new();

		public class CommitDetailDto
		{
			[JsonPropertyName("message")]
			public string Message { get; set; } = string.Empty;

			[JsonPropertyName("committer")]
			public CommitterDto Committer { get; set; } = new();
		}

		public class CommitterDto
		{
			[JsonPropertyName("name")]
			public string Name { get; set; } = string.Empty;
		}
	}

}
