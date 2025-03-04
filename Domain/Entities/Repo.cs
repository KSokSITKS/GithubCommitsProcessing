using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Repo
    {
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Name { get; set; } = string.Empty;
		public string Owner { get; set; } = string.Empty;
	}
}
