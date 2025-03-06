using Application.Github;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace ApplicationUnitTests
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
		public async Task Should_ReturnCommits_When_ApiCallIsSuccessful()
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
			var result = await _sut.GetCommitsFromRepositoryAsync(owner, repoName);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("test-sha", result[0].Sha);
			Assert.AreEqual("test-message", result[0].Commit.Message);
			Assert.AreEqual("test-committer", result[0].Commit.Committer.Name);
		}

		[TestMethod]
		public async Task Should_ReturnEmptyList_When_ApiReturnsNoCommits()
		{
			// Arrange
			var owner = "test-owner";
			var repoName = "test-repo";

			SetupHttpMessageHandler(JsonSerializer.Serialize(new List<GitHubCommitDto>()), HttpStatusCode.OK);

			// Act
			var result = await _sut.GetCommitsFromRepositoryAsync(owner, repoName);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task Should_ThrowGitHubApiException_When_ApiReturnsError()
		{
			// Arrange
			var owner = "test-owner";
			var repoName = "test-repo";
			var expectedUrl = $"repos/{owner}/{repoName}/commits?per_page=100";

			SetupHttpMessageHandler("Not Found", HttpStatusCode.NotFound);

			// Act & Assert
			var exception = await Assert.ThrowsExceptionAsync<GitHubApiException>(async () =>
				await _sut.GetCommitsFromRepositoryAsync(owner, repoName));
			Assert.IsTrue(exception.Message.Contains("Error accessing GitHub API"));

			// Verify the correct URL was called
			VerifyHttpCall(expectedUrl);
		}

		[TestMethod]
		public async Task Should_UseCorrectUrl_When_CallingApi()
		{
			// Arrange
			var owner = "test-owner";
			var repoName = "test-repo";
			var expectedUrl = $"repos/{owner}/{repoName}/commits?per_page=100";

			SetupHttpMessageHandler("[]", HttpStatusCode.OK);

			// Act
			await _sut.GetCommitsFromRepositoryAsync(owner, repoName);

			// Assert
			VerifyHttpCall(expectedUrl);
		}

		[TestMethod]
		public async Task Should_HandlePagination_When_MultiplePages()
		{
			// Arrange
			var owner = "test-owner";
			var repoName = "test-repo";

			// Setup first page response with Link header
			_httpMessageHandlerMock
				.Protected()
				.SetupSequence<Task<HttpResponseMessage>>(
					"SendAsync",
					ItExpr.IsAny<HttpRequestMessage>(),
					ItExpr.IsAny<CancellationToken>()
				)
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent("[{\"sha\":\"commit1\"}]"),
					Headers = {
				{ "Link", "<https://api.github.com/repos/test-owner/test-repo/commits?page=2&per_page=100>; rel=\"next\"" }
					}
				})
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent("[{\"sha\":\"commit2\"}]")
				});

			// Act
			var result = await _sut.GetCommitsFromRepositoryAsync(owner, repoName);

			// Assert
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("commit1", result[0].Sha);
			Assert.AreEqual("commit2", result[1].Sha);

			// Verify both pages were called
			_httpMessageHandlerMock.Protected().Verify(
				"SendAsync",
				Times.Exactly(2),
				ItExpr.Is<HttpRequestMessage>(req =>
					req.Method == HttpMethod.Get &&
					(req.RequestUri.ToString().Contains("?per_page=100") ||
					 req.RequestUri.ToString().Contains("page=2"))),
				ItExpr.IsAny<CancellationToken>()
			);
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
