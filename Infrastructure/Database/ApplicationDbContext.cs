using Domain.Entities;
using System.Data.Entity;

namespace Persistence.Database
{
    public class ApplicationDbContext : DbContext
    {
		public ApplicationDbContext() : base("name=DefaultConnection")
		{
		}

		public DbSet<Repo> Repos { get; set; }
		public DbSet<Commit> Commits { get; set; }
	}
}
