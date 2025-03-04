using Application.Commits;
using Application.Github;
using Application.Repos;
using Domain;
using Domain.Entities;
using Domain.Repositories;
using Moq;

namespace Application.Tests.Repos
{
	[TestClass]
	public class RepoServiceTests
	{
		private Mock<ICommitService> _commitServiceMock;
		private Mock<IRepoRepository> _repoRepositoryMock;
		private Mock<IUnitOfWork> _unitOfWorkMock;
		private Mock<IGitHubService> _gitHubServiceMock;
		private RepoService _sut;

		[TestInitialize]
		public void TestInitialize()
		{
			_commitServiceMock = new Mock<ICommitService>();
			_repoRepositoryMock = new Mock<IRepoRepository>();
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_gitHubServiceMock = new Mock<IGitHubService>();
			_sut = new RepoService(
				_commitServiceMock.Object,
				_repoRepositoryMock.Object,
				_unitOfWorkMock.Object,
				_gitHubServiceMock.Object
			);
		}

		[TestMethod]
		public void Should_ThrowArgumentException_When_RepositoryOwnerIsEmpty()
		{
			// Arrange
			var emptyOwner = string.Empty;
			var repoName = "test-repo";

			// Act & Assert
			var exception = Assert.ThrowsException<ArgumentException>(() =>
				_sut.LoadCommitsToDb(repoName, emptyOwner));
			Assert.AreEqual("Repository owner cannot be empty", exception.Message);
		}

		[TestMethod]
		public void Should_ThrowArgumentException_When_RepositoryNameIsEmpty()
		{
			// Arrange
			var owner = "test-owner";
			var emptyRepoName = string.Empty;

			// Act & Assert
			var exception = Assert.ThrowsException<ArgumentException>(() =>
				_sut.LoadCommitsToDb(emptyRepoName, owner));
			Assert.AreEqual("Repository name cannot be empty", exception.Message);
		}

		[TestMethod]
		public void Should_SaveNewRepo_When_RepoDoesNotExist()
		{
			// Arrange
			var repoName = "test-repo";
			var owner = "test-owner";
			_repoRepositoryMock.Setup(x => x.TryGetRepo(repoName, owner))
				.Returns((Repo?)null);
			_gitHubServiceMock.Setup(x => x.GetCommitsFromRepository(owner, repoName))
				.Returns(new List<GitHubCommitDto>());

			// Act
			_sut.LoadCommitsToDb(repoName, owner);

			// Assert
			_repoRepositoryMock.Verify(x => x.Add(It.Is<Repo>(r =>
				r.Name == repoName &&
				r.Owner == owner)), Times.Once);
		}

		[TestMethod]
		public void Should_NotSaveNewRepo_When_RepoExists()
		{
			// Arrange
			var repoName = "test-repo";
			var owner = "test-owner";
			var existingRepo = new Repo
			{
				Id = Guid.NewGuid(),
				Name = repoName,
				Owner = owner
			};
			_repoRepositoryMock.Setup(x => x.TryGetRepo(repoName, owner))
				.Returns(existingRepo);
			_gitHubServiceMock.Setup(x => x.GetCommitsFromRepository(owner, repoName))
				.Returns(new List<GitHubCommitDto>());

			// Act
			_sut.LoadCommitsToDb(repoName, owner);

			// Assert
			_repoRepositoryMock.Verify(x => x.Add(It.IsAny<Repo>()), Times.Never);
		}

		[TestMethod]
		public void Should_SaveNewCommits_When_CommitsReceived()
		{
			// Arrange
			var repoName = "test-repo";
			var owner = "test-owner";
			var repoId = Guid.NewGuid();
			var existingRepo = new Repo { Id = repoId };
			var githubCommits = new List<GitHubCommitDto>
			{
				new() { Sha = "new-sha", Commit = new() { Message = "test", Committer = new() { Name = "tester" } } }
			};

			_repoRepositoryMock.Setup(x => x.TryGetRepo(repoName, owner))
				.Returns(existingRepo);
			_gitHubServiceMock.Setup(x => x.GetCommitsFromRepository(owner, repoName))
				.Returns(githubCommits);

			// Act
			_sut.LoadCommitsToDb(repoName, owner);

			// Assert
			_commitServiceMock.Verify(x => x.SaveNewCommits(
				It.Is<List<Commit>>(commits =>
					commits.Count == 1 &&
					commits[0].Sha == "new-sha" &&
					commits[0].Message == "test" &&
					commits[0].Committer == "tester"),
				repoId),
				Times.Once);
			_unitOfWorkMock.Verify(x => x.SaveChanges(), Times.Once);
		}

		[TestMethod]
		public void Should_NotProcessCommits_When_NoCommitsReceived()
		{
			// Arrange
			var repoName = "test-repo";
			var owner = "test-owner";
			var repoId = Guid.NewGuid();
			var existingRepo = new Repo { Id = repoId };

			_repoRepositoryMock.Setup(x => x.TryGetRepo(repoName, owner))
				.Returns(existingRepo);
			_gitHubServiceMock.Setup(x => x.GetCommitsFromRepository(owner, repoName))
				.Returns((List<GitHubCommitDto>)null);

			// Act
			_sut.LoadCommitsToDb(repoName, owner);

			// Assert
			_commitServiceMock.Verify(x => x.SaveNewCommits(
				It.IsAny<List<Commit>>(),
				It.IsAny<Guid>()),
				Times.Never);
		}
	}
}
