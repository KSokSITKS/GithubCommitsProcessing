using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Commit
    {
		[Key]
		public string Sha { get; set; } = string.Empty;
		public Guid RepositoryId { get; set; }
        public string Message { get; set; } = string.Empty;
		public string Committer { get; set; } = string.Empty;

		public virtual Repo Repository { get; set; }
	}
}
