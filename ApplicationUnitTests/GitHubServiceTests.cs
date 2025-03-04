using Application.Github;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace Application.Tests.Github
{
	[TestClass]
	public class GitHubServiceTests
	{
		private Mock<IHttpClientFactory> _httpClientFactoryMock;
		private Mock<HttpMessageHandler> _httpMessageHandlerMock;
		private HttpClient _httpClient;
		private GitHubService _sut;

		[TestInitialize]
		public void TestInitialize()
		{
			_httpMessageHandlerMock = new Mock<HttpMessageHandler>();
			_httpClient = new HttpClient(_httpMessageHandlerMock.Object)
			{
				BaseAddress = new Uri("https://api.github.com/")
			};

			_httpClientFactoryMock = new Mock<IHttpClientFactory>();
			_httpClientFactoryMock.Setup(x => x.CreateClient(GitHubClientConfig.Name))
				.Returns(_httpClient);

			_sut = new GitHubService(_httpClientFactoryMock.Object);
		}

		[TestMethod]
		public void Should_ReturnCommits_When_ApiCallIsSuccessful()
		{
			// Arrange
			var owner = "test-owner";
			var repoName = "test-repo";
			var expectedCommits = new List<GitHubCommitDto>
			{
				new()
				{
					Sha = "test-sha",
					Commit = new()
					{
						Message = "test-message",
						Committer = new() { Name = "test-committer" }
					}
				}
			};

			SetupHttpMessageHandler(JsonSerializer.Serialize(expectedCommits), HttpStatusCode.OK);

			// Act
			var result = _sut.GetCommitsFromRepository(owner, repoName);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("test-sha", result[0].Sha);
			Assert.AreEqual("test-message", result[0].Commit.Message);
			Assert.AreEqual("test-committer", result[0].Commit.Committer.Name);
		}

		[TestMethod]
		public void Should_ReturnEmptyList_When_ApiReturnsNoCommits()
		{
			// Arrange
			var owner = "test-owner";
			var repoName = "test-repo";

			SetupHttpMessageHandler(JsonSerializer.Serialize(new List<GitHubCommitDto>()), HttpStatusCode.OK);

			// Act
			var result = _sut.GetCommitsFromRepository(owner, repoName);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public void Should_ThrowGitHubApiException_When_ApiReturnsError()
		{
			// Arrange
			var owner = "test-owner";
			var repoName = "test-repo";

			SetupHttpMessageHandler("Not Found", HttpStatusCode.NotFound);

			// Act & Assert
			var exception = Assert.ThrowsException<GitHubApiException>(() =>
				_sut.GetCommitsFromRepository(owner, repoName));
			Assert.IsTrue(exception.Message.Contains("Error accessing GitHub API"));
		}

		[TestMethod]
		public void Should_UseCorrectUrl_When_CallingApi()
		{
			// Arrange
			var owner = "test-owner";
			var repoName = "test-repo";
			var expectedUrl = $"repos/{owner}/{repoName}/commits";

			SetupHttpMessageHandler("[]", HttpStatusCode.OK);

			// Act
			_sut.GetCommitsFromRepository(owner, repoName);

			// Assert
			VerifyHttpCall(expectedUrl);
		}

		private void SetupHttpMessageHandler(string content, HttpStatusCode statusCode)
		{
			_httpMessageHandlerMock
				.Protected()
				.Setup<Task<HttpResponseMessage>>(
					"SendAsync",
					ItExpr.IsAny<HttpRequestMessage>(),
					ItExpr.IsAny<CancellationToken>()
				)
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = statusCode,
					Content = new StringContent(content)
				});
		}

		private void VerifyHttpCall(string expectedUrl)
		{
			_httpMessageHandlerMock
				.Protected()
				.Verify(
					"SendAsync",
					Times.Once(),
					ItExpr.Is<HttpRequestMessage>(req =>
						req.Method == HttpMethod.Get &&
						req.RequestUri.ToString() == $"https://api.github.com/{expectedUrl}"),
					ItExpr.IsAny<CancellationToken>()
				);
		}
	}
}
