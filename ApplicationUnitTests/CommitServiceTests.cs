using Application.Commits;
using Domain.Entities;
using Domain.Repositories;
using Moq;

namespace ApplicationUnitTests
{
	[TestClass]
	public class CommitServiceTests
	{
		private Mock<ICommitRepository> _commitRepositoryMock;
		private CommitService _sut;

		[TestInitialize]
		public void TestInitialize()
		{
			_commitRepositoryMock = new Mock<ICommitRepository>();
			_sut = new CommitService(_commitRepositoryMock.Object);
		}

		[TestMethod]
		public void Should_NotSaveCommits_When_CommitListIsEmpty()
		{
			// Arrange
			var emptyCommits = new List<Commit>();
			var repoId = Guid.NewGuid();

			// Act
			_sut.SaveNewCommits(emptyCommits, repoId);

			// Assert
			_commitRepositoryMock.Verify(x => x.GetCommitsForRepository(It.IsAny<Guid>()), Times.Never);
			_commitRepositoryMock.Verify(x => x.AddMany(It.IsAny<List<Commit>>()), Times.Never);
		}

		[TestMethod]
		public void Should_SaveAllCommits_When_NoExistingCommitsInRepository()
		{
			// Arrange
			var repoId = Guid.NewGuid();
			var newCommits = new List<Commit>
			{
				new() { Sha = "sha1", Message = "message1", Committer = "committer1" },
				new() { Sha = "sha2", Message = "message2", Committer = "committer2" }
			};

			_commitRepositoryMock.Setup(x => x.GetCommitsForRepository(repoId))
				.Returns(new List<Commit>());

			// Act
			_sut.SaveNewCommits(newCommits, repoId);

			// Assert
			_commitRepositoryMock.Verify(x => x.AddMany(It.Is<List<Commit>>(
				commits => commits.Count == 2 &&
						  commits.Any(c => c.Sha == "sha1") &&
						  commits.Any(c => c.Sha == "sha2"))), Times.Once);
		}

		[TestMethod]
		public void Should_SaveOnlyNewCommits_When_SomeCommitsAlreadyExist()
		{
			// Arrange
			var repoId = Guid.NewGuid();
			var existingCommits = new List<Commit>
			{
				new() { Sha = "sha1", Message = "message1", Committer = "committer1" }
			};

			var newCommits = new List<Commit>
			{
				new() { Sha = "sha1", Message = "message1", Committer = "committer1" },
				new() { Sha = "sha2", Message = "message2", Committer = "committer2" }
			};

			_commitRepositoryMock.Setup(x => x.GetCommitsForRepository(repoId))
				.Returns(existingCommits);

			// Act
			_sut.SaveNewCommits(newCommits, repoId);

			// Assert
			_commitRepositoryMock.Verify(x => x.AddMany(It.Is<List<Commit>>(
				commits => commits.Count == 1 &&
						  commits.All(c => c.Sha == "sha2"))), Times.Once);
		}

		[TestMethod]
		public void Should_NotSaveAnyCommits_When_AllCommitsAlreadyExist()
		{
			// Arrange
			var repoId = Guid.NewGuid();
			var existingCommits = new List<Commit>
			{
				new() { Sha = "sha1", Message = "message1", Committer = "committer1" },
				new() { Sha = "sha2", Message = "message2", Committer = "committer2" }
			};

			var newCommits = new List<Commit>
			{
				new() { Sha = "sha1", Message = "message1", Committer = "committer1" },
				new() { Sha = "sha2", Message = "message2", Committer = "committer2" }
			};

			_commitRepositoryMock.Setup(x => x.GetCommitsForRepository(repoId))
				.Returns(existingCommits);

			// Act
			_sut.SaveNewCommits(newCommits, repoId);

			// Assert
			_commitRepositoryMock.Verify(x => x.GetCommitsForRepository(repoId), Times.Once);
			_commitRepositoryMock.Verify(x => x.AddMany(It.IsAny<List<Commit>>()), Times.Never);
		}
	}
}
